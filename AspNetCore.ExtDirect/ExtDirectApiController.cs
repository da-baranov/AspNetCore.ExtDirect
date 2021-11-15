using AspNetCore.ExtDirect.Meta;
using AspNetCore.ExtDirect.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// We do not use routing attributes intentionally.
    /// Use IServiceCollection.AddExtDirect... and IApplicationBuilder.UseExtDirect extension methods to define controller routes
    /// </remarks>
    public class ExtDirectApiController : Controller
    {
        private readonly ExtDirectOptions _options;
        private readonly ExtDirectHandlerRepository _repository;
        private readonly IServiceProvider _serviceProvider;

        public ExtDirectApiController(IServiceProvider serviceProvider,
                                      ExtDirectHandlerRepository repository,
                                      ExtDirectOptions options)
        {
            _serviceProvider = serviceProvider 
                ?? throw new ArgumentNullException(nameof(serviceProvider));
            _repository = repository 
                ?? throw new ArgumentNullException(nameof(repository));
            _options = options 
                ?? throw new ArgumentNullException(nameof(options));
        }

        [AcceptVerbs("GET")]
        public async Task<IActionResult> Events()
        {
            var result = await OnEvents();
            return Ok(result);
        }

        [AcceptVerbs("GET")]
        public async Task<IActionResult> Index()
        {
            return await OnGet();
        }

        [AcceptVerbs("POST")]
        public async Task<IActionResult> Index(
            [FromBody]
            [ModelBinder(typeof(ExtDirectRequestModelBinder))] 
            RemotingRequestBatch request)
        {
            var handler = new ExtDirectActionHandler(_serviceProvider, request);
            var result = await handler.ExecuteAsync();
            return Ok(result);
        }

        private async Task<List<PollResponse>> OnEvents()
        {
            var handler = new ExtDirectPollingEventHandler(_serviceProvider);
            var result = await handler.ExecuteAsync();
            return result;
        }

        private async Task<IActionResult> OnGet()
        {
            var script = new StringBuilder();

            script.AppendLine($"var Ext = Ext || {{}};");

            // Remoting API
            script.AppendLine($"Ext.{_options.RemotingApiName} = ");
            var remotingApi = _repository.ToRemotingApi();
            remotingApi.Id = _options.RemotingApiId;
            remotingApi.Url = this.Url.Content("~/" + _options.RemotingRouteUrl);
            script.AppendLine(Util.JsonSerialize(remotingApi));
            script.AppendLine(";");
            script.AppendLine();

            // Polling API
            var pollingApi = new PollingApi
            {
                Id = _options.PollingApiId,
                Url = this.Url.Content("~/" + _options.PollingRouteUrl)
            };
            script.AppendLine($"Ext.{_options.PollingApiName} = ");
            script.Append(Util.JsonSerialize(pollingApi));
            script.Append(';');

            var result = Content(script.ToString(), "text/javascript;charset=UTF-8");
            return await Task.FromResult(result);
        }
    }
}