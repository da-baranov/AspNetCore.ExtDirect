using AspNetCore.ExtDirect.Meta;
using AspNetCore.ExtDirect.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect
{
    /// <summary>
    /// Executes Ext Direct remoting requests
    /// </summary>
    internal sealed class ExtDirectRemotingHandler
    {
        private readonly RemotingRequestBatch _batch;
        private readonly IStringLocalizer _localizer;
        private readonly IStringLocalizerFactory _localizerFactory;
        private readonly string _providerId;
        private readonly ExtDirectHandlerRepository _repository;
        private readonly IServiceProvider _serviceProvider;
        private readonly ExtDirectBatchService _batchService;
        private readonly ExtDirectOptions _options;
        private readonly ILogger<ExtDirectRemotingHandler> _logger;
        private List<RemotingResponseBase> _result;


        /// <summary>
        /// Constructs an instance of ExtDirectActionHandler
        /// </summary>
        /// <param name="serviceProvider">ASP.NET Core web application service provider</param>
        /// <param name="providerId">Name of Ext Direct remoting provider</param>
        /// <param name="batch">Batch to be executed</param>
        internal ExtDirectRemotingHandler(IServiceProvider serviceProvider, string providerId, RemotingRequestBatch batch)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _providerId = providerId;
            _batch = batch ?? throw new ArgumentNullException(nameof(batch));
            _options = serviceProvider.GetRequiredService<ExtDirectOptions>();
            _repository = serviceProvider.GetService<ExtDirectHandlerRepository>();
            _logger = serviceProvider.GetService<ILogger<ExtDirectRemotingHandler>>();

            _batchService = (ExtDirectBatchService)serviceProvider.GetService<IExtDirectBatchService>();

            _localizerFactory = serviceProvider.GetService<IStringLocalizerFactory>();
            _localizer = _localizerFactory.Create(typeof(Properties.Resources));
        }

        /// <summary>
        /// Executes the Ext Direct batch passed as constructor argument
        /// </summary>
        /// <returns></returns>
        internal async Task<List<RemotingResponseBase>> ExecuteAsync()
        {
            await ValidateBatchAsync();
            await ProcessBatchAsync(_providerId);
            return _result;
        }

        private async Task<object> InvokeAsync(RemotingRequest request,       // Batch item
                                               RemotingAction remotingAction, // Action descriptor
                                               RemotingMethod remotingMethod, // Method descriptor
                                               object obj,                    // Handler
                                               MethodInfo methodInfo,         // Handler method to be invoked
                                               object arguments)              // Handler method arguments (as JToken)
        {
            try
            {
                var parameters = methodInfo.GetParameters();

                // Form POST/Upload handler?
                if (remotingMethod.FormHandler)
                {
                    var args = new List<object>();
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        var parameterType = parameters[i].ParameterType;
                        if (typeof(IEnumerable<IFormFile>).IsAssignableFrom(parameterType))
                        {
                            args.Add(request.FormFiles);
                        }
                        else
                        {
                            if (arguments == null)
                            {
                                args.Add(null);
                            }
                            else if (arguments is JToken jtoken)
                            {
                                var arg = jtoken.ToObject(parameterType);
                                args.Add(arg);
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                    }
                    return await Util.InvokeSyncOrAsync(obj, methodInfo, args.ToArray());
                }

                else
                {
                    // Ordered arguments
                    if (arguments is JArray array)
                    {
                        var args = new List<object>();

                        // There is no way to distinguish between an array that contains named arguments and array of objects sent by ExtJS store
                        // If methodInfo has exactly 1 parameter of type IEnumerable, the array must be handled as Store request
                        if (array.Count > 0
                            && array[0] is JObject
                            && parameters.Length == 1
                            && typeof(IEnumerable).IsAssignableFrom(parameters[0].ParameterType))
                        {
                            var value = array.ToObject(parameters[0].ParameterType);
                            args.Add(value);
                        }
                        else
                        {
                            for (var i = 0; i < parameters.Length; i++)
                            {
                                if (i < array.Count)
                                {
                                    var parameter = parameters[i];
                                    var parameterType = parameter.ParameterType;
                                    var value = array[i].ToObject(parameterType);
                                    args.Add(value);
                                }
                                else
                                {
                                    args.Add(null);
                                }
                            }
                        }
                        return await Util.InvokeSyncOrAsync(obj, methodInfo, args.ToArray());
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
                        return await Util.InvokeSyncOrAsync(obj, methodInfo, args.ToArray());
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
                        return await Util.InvokeSyncOrAsync(obj, methodInfo, args.ToArray());
                    }
                    else if (request.Data == null)
                    {
                        return await Util.InvokeSyncOrAsync(obj, methodInfo);
                    }
                    else
                    {
                        throw new NotSupportedException(_localizer[nameof(Properties.Resources.ERR_INVALID_JSON),
                                request.Action,
                                request.Method,
                                request.Data?.GetType()]);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to invoke remoting handler method (action: {0}, method: {1})", remotingAction.Name, remotingMethod.Name);
                throw;
            }
        }

        /// <summary>
        /// Executes Ext Direct batch
        /// </summary>
        /// <returns>Nothing</returns>
        private async Task ProcessBatchAsync(string providerId)
        {
            var transactionId = Util.Uuid();
            _batchService.FireBatchBegin(transactionId);

            try
            {
                _result = new List<RemotingResponseBase>();
                foreach (var batchItem in _batch)
                {
                    var rv = await ProcessSingleBatchItemAsync(providerId, batchItem);
                    _result.Add(rv);
                }
                _batchService.FireBatchCommit(transactionId);
            }
            catch (Exception ex)
            {
                _batchService.FireBatchRollback(transactionId, ex);
                throw;
            }
        }

        /// <summary>
        /// Executes a single Ext Direct batch item
        /// </summary>
        /// <param name="request"></param>
        /// <returns>RemotingResponse or RemotingException</returns>
        private async Task<RemotingResponseBase> ProcessSingleBatchItemAsync(string providerId, RemotingRequest request)
        {
            try
            {
                // Looking for an Action handler in repository
                _repository.FindExtDirectActionAndMethod(providerId, request.Action, request.Method, out RemotingAction remotingAction, out RemotingMethod remotingMethod, out Type type, out MethodInfo methodInfo);

                // Create an instance of handler service using DI
                var instance = ActivatorUtilities.CreateInstance(_serviceProvider, type);

                // Invoke handler method
                var data = await InvokeAsync(request, remotingAction, remotingMethod, instance, methodInfo, request.Data);

                var response = new RemotingResponse(request, data);
                return await Task.FromResult(response);
            }
            catch (TargetInvocationException tex)
            {
                if (tex.InnerException != null)
                {
                    var ex = new RemotingException(request, tex.InnerException);
                    if (_options.Debug == false) ex.Where = null;
                    return await Task.FromResult(ex);
                }
                else
                {
                    var ex = new RemotingException(request, tex);
                    if (_options.Debug == false) ex.Where = null;
                    return await Task.FromResult(ex);
                }
            }
            catch (Exception ex)
            {
                var exception = new RemotingException(ex, request.Action, request.Method, request.Tid);
                if (_options.Debug == false) exception.Where = null;
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