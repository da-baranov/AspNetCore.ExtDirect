using AspNetCore.ExtDirect.Meta;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
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

        public ExtDirectPollingEventHandler(IServiceProvider serviceProvider, string providerName)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _providerName = !string.IsNullOrWhiteSpace(providerName) ? providerName : throw new ArgumentNullException(nameof(providerName));
            _localizerFactory = serviceProvider.GetService<IStringLocalizerFactory>();
            _localizer = _localizerFactory.Create(typeof(Properties.Resources));
            _repository = _serviceProvider.GetService<ExtDirectHandlerRepository>();
        }

        public async Task<List<PollResponse>> ExecuteAsync()
        {
            if (!_repository.PollingApis.TryGetValue(_providerName, out PollingApi pollingApi))
            {
                throw new Exception(_localizer[nameof(Properties.Resources.ERR_CANNOT_FIND_POLLING_HANDLER), _providerName]);
            }

            var result = new List<PollResponse>();
            foreach (var pollingHandler in pollingApi.HandlerTypes)
            {
                var handlerInstance = ActivatorUtilities.CreateInstance(_serviceProvider, pollingHandler) as IExtDirectPollingEventSource;
                // null checking is not required
                var events = handlerInstance.GetEvents();
                if (events != null)
                {
                    result.AddRange(events);
                }
            }

            return await Task.FromResult(result);
        }
    }
}