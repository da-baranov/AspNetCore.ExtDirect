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
    internal sealed class ExtDirectPollingEventHandler
    {
        private readonly IStringLocalizer _localizer;
        private readonly IStringLocalizerFactory _localizerFactory;
        private readonly string _providerName;
        private readonly ExtDirectHandlerRepository _repository;
        private readonly IServiceProvider _serviceProvider;
        private readonly ControllerContext _controllerContext;

        internal ExtDirectPollingEventHandler(IServiceProvider serviceProvider,
                                              ControllerContext controllerContext,
                                              string providerName)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _controllerContext = controllerContext ?? throw new ArgumentNullException(nameof(controllerContext));
            _providerName = !string.IsNullOrWhiteSpace(providerName) ? providerName : throw new ArgumentNullException(nameof(providerName));

            _localizerFactory = serviceProvider.GetService<IStringLocalizerFactory>();
            _localizer = _localizerFactory.Create(typeof(Properties.Resources));
            _repository = _serviceProvider.GetService<ExtDirectHandlerRepository>();
        }

        public async Task<IEnumerable<PollResponse>> ExecuteAsync()
        {
            if (!_repository.PollingApis.TryGetValue(_providerName, out PollingApi pollingApi))
            {
                throw new Exception(_localizer[nameof(Properties.Resources.ERR_CANNOT_FIND_POLLING_HANDLER), _providerName]);
            }

            var result = new List<PollResponse>();

            foreach (var pollingHandlerType in pollingApi.HandlerTypes.Keys)
            {
                // Creates an instance of polling event handler using dependency injection
                var handlerInstance = ActivatorUtilities.CreateInstance(_serviceProvider, pollingHandlerType);
                var tmp = pollingApi.HandlerTypes[pollingHandlerType];

                var handlerMethodInfoType = tmp.GetType();

                var handlerMethodInfo = handlerMethodInfoType.GetMethod("Invoke");

                var handlerMethodInfoParameters = handlerMethodInfo.GetParameters().ToList();

                if (handlerMethodInfoParameters.Count == 1)
                {
                    // obj argument is func itself (pointer to the polling event source non-static method)
                    // [0] argument is an instance of polling events source
                    result.AddRange(handlerMethodInfo.Invoke(tmp, new object[] { handlerInstance }) as IEnumerable<PollResponse>);
                }

                else if (handlerMethodInfoParameters.Count == 2)
                {
                    var argumentType = handlerMethodInfoParameters[1].ParameterType;
                    var modelBinder = new ExtDirectGenericModelBinder(_serviceProvider, _controllerContext);
                    var args = await modelBinder.BindAsync(argumentType);
                    // obj argument is func itself (pointer to the polling event source non-static method)
                    // [0] argument is an instance of polling events source
                    // [1] argument is a polling handler method argument
                    result.AddRange(handlerMethodInfo.Invoke(tmp, new object[] { handlerInstance, args }) as IEnumerable<PollResponse>);
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