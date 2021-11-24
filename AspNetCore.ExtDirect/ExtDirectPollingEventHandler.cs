using AspNetCore.ExtDirect.Meta;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private readonly QueryString _queryString;
        private readonly IModelBinderFactory _modelBinderFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        internal ExtDirectPollingEventHandler(IServiceProvider serviceProvider, 
                                              string providerName,
                                              QueryString queryString)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _providerName = !string.IsNullOrWhiteSpace(providerName) ? providerName : throw new ArgumentNullException(nameof(providerName));
            _queryString = queryString;

            _localizerFactory = serviceProvider.GetService<IStringLocalizerFactory>();
            _localizer = _localizerFactory.Create(typeof(Properties.Resources));
            _repository = _serviceProvider.GetService<ExtDirectHandlerRepository>();
            _modelBinderFactory = serviceProvider.GetService<IModelBinderFactory>();
            _httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
        }

        public async Task<IEnumerable<PollResponse>> ExecuteAsync()
        {
            if (!_repository.PollingApis.TryGetValue(_providerName, out PollingApi pollingApi))
            {
                throw new Exception(_localizer[nameof(Properties.Resources.ERR_CANNOT_FIND_POLLING_HANDLER), _providerName]);
            }

            var result = default(IEnumerable<PollResponse>);
            foreach (var pollingHandlerType in pollingApi.HandlerTypes.Keys)
            {
                var handlerInstance = ActivatorUtilities.CreateInstance(_serviceProvider, pollingHandlerType);
                var tmp = pollingApi.HandlerTypes[pollingHandlerType];
                var handlerMethodInfoType = tmp.GetType();
                var handlerMethodInfo = handlerMethodInfoType.GetMethod("Invoke");
                var handlerMethodInfoParameters = handlerMethodInfo.GetParameters().ToList();

                
                if (handlerMethodInfoParameters.Count == 1) // the "sender" argument is func, [0] argument is polling events source
                {
                    result = handlerMethodInfo.Invoke(tmp, new object[] { handlerInstance }) as IEnumerable<PollResponse>;
                }
                else if (handlerMethodInfoParameters.Count == 2)
                {
                    result = null;
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