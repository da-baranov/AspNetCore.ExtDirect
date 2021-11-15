using AspNetCore.ExtDirect.Meta;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect
{
    internal sealed class ExtDirectPollingEventHandler
    {
        private IServiceProvider _serviceProvider;
        private ExtDirectHandlerRepository _repository;
        private IStringLocalizerFactory _localizerFactory;
        private IStringLocalizer _localizer;
        private List<PollResponse> _result = new();

        public ExtDirectPollingEventHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _repository = serviceProvider.GetService<ExtDirectHandlerRepository>();

            _localizerFactory = serviceProvider.GetService<IStringLocalizerFactory>();
            _localizer = _localizerFactory.Create(typeof(Properties.Resources));
        }

        public async Task<List<PollResponse>> ExecuteAsync()
        {
            _result.Clear();
            foreach (var type in _repository.PollingHandlers)
            {
                var handlerInstance = ActivatorUtilities.CreateInstance(_serviceProvider, type) as IExtDirectPollingEventSource;
                _result.AddRange(handlerInstance.GetEvents());
            }
            return await Task.FromResult(_result);
        }
    }
}