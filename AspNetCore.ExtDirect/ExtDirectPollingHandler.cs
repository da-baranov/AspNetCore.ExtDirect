using AspNetCore.ExtDirect.Binders;
using AspNetCore.ExtDirect.Meta;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
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
        private readonly ControllerContext _controllerContext;
        private readonly IStringLocalizer _localizer;
        private readonly IStringLocalizerFactory _localizerFactory;
        private readonly ILogger<ExtDirectPollingHandler> _logger;
        private readonly string _providerId;
        private readonly ExtDirectHandlerRepository _repository;
        private readonly IServiceProvider _serviceProvider;

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
            _logger = _serviceProvider.GetService<ILogger<ExtDirectPollingHandler>>();
        }

        internal async Task<IEnumerable<PollResponse>> ExecuteAsync()
        {
            var pollingApi = default(PollingApi);

            try
            {
                // Lookup provider by ID
                if (!_repository.PollingApis.TryGetValue(_providerId, out pollingApi))
                {
                    throw new Exception(_localizer[nameof(Properties.Resources.ERR_CANNOT_FIND_API_BY_ID), _providerId]);
                }

                var result = new List<PollResponse>();

                foreach (var handlerRegistryItem in pollingApi.HandlerTypes)
                {
                    // Creates an instance of polling event handler using dependency injection
                    var handlerInstance = ActivatorUtilities.CreateInstance(_serviceProvider, handlerRegistryItem.HandlerType);

                    var func = handlerRegistryItem.Func;                                // Func<>
                    var funcMethodInfo = func.GetType().GetMethod("Invoke");            // Polling handler method info (e.g.SomePollingHandler.GetEvents)
                    var funcMethodParameters = funcMethodInfo.GetParameters().ToList(); // Polling handler method parameters
                    var eventName = handlerRegistryItem.EventName;                      // Event name (from configuration or attribute)

                    if (funcMethodParameters.Count == 1) // Handler method accepts no parameters
                    {
                        // obj argument is func itself (pointer to a polling event source non-static method)
                        // [0] argument is an instance of polling events source
                        var rv = (IEnumerable)await Utils.Util.InvokeSyncOrAsync(func, funcMethodInfo, new object[] { handlerInstance });

                        if (rv != null)
                        {
                            foreach (var obj in rv)
                            {
                                result.Add(new PollResponse { Name = eventName, Data = obj });
                            }
                        }
                    }
                    else if (funcMethodParameters.Count == 2) // Handler method accepts exactly one parameter
                    {
                        var argumentType = funcMethodParameters[1].ParameterType;
                        var modelBinder = new ExtDirectGenericModelBinder(_serviceProvider, _controllerContext);
                        var args = await modelBinder.BindAsync(argumentType);
                        // obj argument is func itself (pointer to a polling event source non-static method)
                        // [0] argument is an instance of polling events source
                        // [1] argument is a polling handler method argument
                        var rv = (IEnumerable)await Utils.Util.InvokeSyncOrAsync(func, funcMethodInfo, new object[] { handlerInstance, args });
                        if (rv != null)
                        {
                            foreach (var obj in rv)
                            {
                                result.Add(new PollResponse { Name = eventName, Data = obj });
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(_localizer[nameof(Properties.Resources.ERR_POLLING_HANDLER_TOO_MANY_PARAMETERS)]);
                    }
                }

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Failed to invoke polling handler with ID {0}", pollingApi?.Id);
                throw;
            }
        }
    }
}