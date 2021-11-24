﻿using AspNetCore.ExtDirect.Meta;
using AspNetCore.ExtDirect.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect
{
    /// <summary>
    /// Executes Ext Direct remoting requests
    /// </summary>
    internal sealed class ExtDirectActionHandler
    {
        private readonly RemotingRequestBatch _batch;
        private readonly IStringLocalizer _localizer;
        private readonly IStringLocalizerFactory _localizerFactory;
        private readonly string _providerName;
        private readonly ExtDirectHandlerRepository _repository;
        private readonly IServiceProvider _serviceProvider;
        private readonly ExtDirectTransactionService _transactionService;
        private List<object> _result;

        internal ExtDirectActionHandler(IServiceProvider serviceProvider, string providerName, RemotingRequestBatch batch)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _providerName = providerName;
            _batch = batch ?? throw new ArgumentNullException(nameof(batch));

            _repository = serviceProvider.GetService<ExtDirectHandlerRepository>();
            _transactionService = serviceProvider.GetService<ExtDirectTransactionService>();

            _localizerFactory = serviceProvider.GetService<IStringLocalizerFactory>();
            _localizer = _localizerFactory.Create(typeof(Properties.Resources));
        }

        internal async Task<List<object>> ExecuteAsync()
        {
            await ValidateBatchAsync();
            await ProcessBatchAsync(_providerName);
            return _result;
        }

        /// <summary>
        /// Invokes an object's method
        /// </summary>
        /// <param name="obj">An object</param>
        /// <param name="methodInfo">Object method descriptor</param>
        /// <param name="arguments">Arguments in JSON format. This can be either a scalar, a javascript array, or a javascript object</param>
        /// <returns></returns>
        private async Task<object> InvokeAsync(RemotingRequest request, object obj, MethodInfo methodInfo, object arguments)
        {
            async Task<object> _InvokeAsync(params object[] args)
            {
                var task = (Task)methodInfo.Invoke(obj, args);
                await task.ConfigureAwait(false);
                var resultProperty = task.GetType().GetProperty("Result");

                if (resultProperty == null) return null;

                var rv = resultProperty.GetValue(task);
                return rv;
            }

            async Task<object> _InvokeSync(params object[] args)
            {
                var result = methodInfo.Invoke(obj, args);
                return await Task.FromResult(result);
            }

            // ***

            var isAsync =
                methodInfo.GetCustomAttribute<AsyncStateMachineAttribute>(false) != null ||
                methodInfo.Name.EndsWith("async", StringComparison.InvariantCultureIgnoreCase);

            var parameters = methodInfo.GetParameters();

            // Ordered arguments
            if (arguments is JArray array)
            {
                var args = new List<object>();
                for (var i = 0; i < parameters.Length; i++)
                {
                    if (i < array.Count)
                    {
                        var parameter = parameters[i];
                        var parameterType = parameter.ParameterType;
                        var value = array[i].ToObject(parameterType);
                        args.Add(value);
                    }
                }
                return isAsync ?
                    await _InvokeAsync(args.ToArray()) :
                    await _InvokeSync(args.ToArray());
            }

            // Named arguments
            else if (arguments is JObject jobject)
            {
                var args = new List<object>();
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    var parameterType = parameter.ParameterType;
                    if (jobject.TryGetValue(parameter.Name, StringComparison.InvariantCultureIgnoreCase, out JToken jvalue))
                    {
                        args.Add(jvalue.ToObject(parameterType));
                    }
                    else
                    {
                        args.Add(null);
                    }
                }
                return isAsync ?
                    await _InvokeAsync(args.ToArray()) :
                    await _InvokeSync(args.ToArray());
            }

            // Scalar?
            else if (arguments is JToken jtoken)
            {
                var args = new List<object>();
                if (parameters.Length > 0)
                {
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        if (i == 0)
                        {
                            var parameter = parameters[i];
                            var parameterType = parameter.ParameterType;
                            var value = jtoken.ToObject(parameterType);
                            args.Add(value);
                        }
                        else
                        {
                            args.Add(null);
                        }
                    }
                }
                return isAsync ?
                    await _InvokeAsync(args.ToArray()) :
                    await _InvokeSync(args.ToArray());
            }
            else if (request.Data == null)
            {
                return isAsync ?
                    await _InvokeAsync(Array.Empty<object>()) :
                    await _InvokeSync(Array.Empty<object>());
            }
            else
            {
                throw new NotSupportedException(_localizer[nameof(Properties.Resources.ERR_INVALID_JSON),
                        request.Action,
                        request.Method,
                        request.Data?.GetType()]);
            }
        }

        /// <summary>
        /// Executes Ext Direct batch
        /// </summary>
        /// <returns>Nothing</returns>
        private async Task ProcessBatchAsync(string providerName)
        {
            var transactionId = Util.Uuid();
            _transactionService.FireTransactionBegin(transactionId);

            try
            {
                _result = new List<object>();
                foreach (var batchItem in _batch)
                {
                    var rv = await ProcessSingleBatchItemAsync(providerName, batchItem);
                    _result.Add(rv);
                }
                _transactionService.FireTransactionCommit(transactionId);
            }
            catch (Exception ex)
            {
                _transactionService.FireTransactionRollback(transactionId, ex);
                throw;
            }
        }

        /// <summary>
        /// Executes a single Ext Direct batch item
        /// </summary>
        /// <param name="request"></param>
        /// <returns>RemotingResponse or RemotingException</returns>
        private async Task<object> ProcessSingleBatchItemAsync(string providerName, RemotingRequest request)
        {
            try
            {
                // Looking for an Action handler in repository
                _repository.FindExtDirectActionAndMethod(providerName, request.Action, request.Method, out Type type, out MethodInfo methodInfo);

                // Create an instance of handler service using DI
                var instance = ActivatorUtilities.CreateInstance(_serviceProvider, type);

                // Invoke handler method
                var data = await InvokeAsync(request, instance, methodInfo, request.Data);

                var response = new RemotingResponse(request, data);
                return await Task.FromResult(response);
            }
            catch (TargetInvocationException tex)
            {
                if (tex.InnerException != null)
                {
                    var ex = new RemotingException(request, tex.InnerException);
                    return await Task.FromResult(ex);
                }
                else
                {
                    var ex = new RemotingException(request, tex);
                    return await Task.FromResult(ex);
                }
            }
            catch (Exception ex)
            {
                var exception = new RemotingException(ex, request.Action, request.Method, request.Tid);
                return await Task.FromResult(exception);
            }
        }
        /// <summary>
        /// Performs simple batch validation
        /// </summary>
        /// <returns></returns>
        private async Task ValidateBatchAsync()
        {
            var validator = new RemotingRequestValidator();
            foreach (var batchItem in _batch)
            {
                await validator.ValidateAsync(batchItem);
            }
        }
    }
}