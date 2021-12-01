using AspNetCore.ExtDirect.Binders;
using AspNetCore.ExtDirect.Meta;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect
{
    /// <summary>
    /// Handles Ext Direct polling requests
    /// </summary>
    internal sealed class ExtDirectPollingHandler
    {
        private readonly IStringLocalizer _localizer;
        private readonly IStringLocalizerFactory _localizerFactory;
        private readonly string _providerId;
        private readonly ExtDirectHandlerRepository _repository;
        private readonly IServiceProvider _serviceProvider;
        private readonly ControllerContext _controllerContext;

        internal ExtDirectPollingHandler(IServiceProvider serviceProvider,
                                              ControllerContext controllerContext,
                                              string providerId)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _controllerContext = controllerContext ?? throw new ArgumentNullException(nameof(controllerContext));
            _providerId = !string.IsNullOrWhiteSpace(providerId) ? providerId : throw new ArgumentNullException(nameof(providerId));

            _localizerFactory = serviceProvider.GetService<IStringLocalizerFactory>();
            _localizer = _localizerFactory.Create(typeof(Properties.Resources));
            _repository = _serviceProvider.GetService<ExtDirectHandlerRepository>();
        }

        public async Task<IEnumerable<PollResponse>> ExecuteAsync()
        {
            if (!_repository.PollingApis.TryGetValue(_providerId, out PollingApi pollingApi))
            {
                throw new Exception(_localizer[nameof(Properties.Resources.ERR_CANNOT_FIND_API_BY_ID), _providerId]);
            }

            var result = new List<PollResponse>();

            foreach (var pollingHandlerType in pollingApi.HandlerTypes.Keys)
            {
                // Creates an instance of polling event handler using dependency injection
                var handlerInstance = ActivatorUtilities.CreateInstance(_serviceProvider, pollingHandlerType);

                var tmp = pollingApi.HandlerTypes[pollingHandlerType]; // Func<> (delegate)

                var handlerMethodInfoType = tmp.GetType(); // typeof(Func<>)

                var handlerMethodInfo = handlerMethodInfoType.GetMethod("Invoke"); // Polling handler method info (e.g. SomePollingHandler.GetEvents)

                var handlerMethodInfoParameters = handlerMethodInfo.GetParameters().ToList();

                if (handlerMethodInfoParameters.Count == 1)
                {
                    // obj argument is func itself (pointer to the polling event source non-static method)
                    // [0] argument is an instance of polling events source
                    var rv = await Utils.Util.InvokeSyncOrAsync(tmp, handlerMethodInfo, new object[] { handlerInstance }) as IEnumerable<PollResponse>;
                    //result.AddRange(handlerMethodInfo.Invoke(tmp, new object[] { handlerInstance }) as IEnumerable<PollResponse>);
                    result.AddRange(rv);
                }

                else if (handlerMethodInfoParameters.Count == 2)
                {
                    var argumentType = handlerMethodInfoParameters[1].ParameterType;
                    var modelBinder = new ExtDirectGenericModelBinder(_serviceProvider, _controllerContext);
                    var args = await modelBinder.BindAsync(argumentType);
                    // obj argument is func itself (pointer to the polling event source non-static method)
                    // [0] argument is an instance of polling events source
                    // [1] argument is a polling handler method argument
                    // result.AddRange(handlerMethodInfo.Invoke(tmp, new object[] { handlerInstance, args }) as IEnumerable<PollResponse>);
                    var rv = await Utils.Util.InvokeSyncOrAsync(tmp, handlerMethodInfo, new object[] { handlerInstance, args });
                    var rv1 = rv as IEnumerable<PollResponse>;
                    result.AddRange(rv1);
                }
                else
                {
                    throw new Exception(_localizer[nameof(Properties.Resources.ERR_POLLING_HANDLER_TOO_MANY_PARAMETERS)]);
                }
            }

            return await Task.FromResult(result);
        }
    }
}