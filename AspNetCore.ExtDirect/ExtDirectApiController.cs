using AspNetCore.ExtDirect.Binders;
using AspNetCore.ExtDirect.Meta;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect
{
    /// <summary>
    /// Provides methods that respond to Ext Direct requests
    /// </summary>
    /// <remarks>
    /// Routing attributes are not used, intentionally.
    /// Use extension methods to configure AspNetCore.ExtDirect services and routes on application Startup
    /// </remarks>
    public class ExtDirectApiController : Controller
    {
        private readonly ExtDirectOptions _options;
        private readonly ExtDirectHandlerRepository _repository;
        private readonly IServiceProvider _serviceProvider;

        public ExtDirectApiController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            _options = serviceProvider.GetRequiredService<ExtDirectOptions>();
            _repository = serviceProvider.GetRequiredService<ExtDirectHandlerRepository>();
        }

        /// <summary>
        /// Creates Ext Direct client JavaScript API descriptor
        /// </summary>
        /// <remarks>A reference to this API must be known to ExtJS client application, e.g. &lt;script src="~/ExtDirect.js"&gt;></remarks>
        /// <see href="https://docs.sencha.com/extjs/7.0.0/guides/backend_connectors/direct/specification.html"/>
        [AcceptVerbs("GET")]
        [Produces(ExtDirectConstants.CONTENT_TYPE_TEXT_JAVASCRIPT)]
        public async Task<IActionResult> Index()
        {
            var result = _repository.ToApi(Url, _options);
            return await Task.FromResult(Content(result, ExtDirectConstants.CONTENT_TYPE_TEXT_JAVASCRIPT, Encoding.UTF8));
        }

        /// <summary>
        /// Handles client Ext Direct remoting requests
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="request"></param>
        /// <see href="https://docs.sencha.com/extjs/7.0.0/guides/backend_connectors/direct/specification.html"/>
        [AcceptVerbs("POST")]
        [Consumes(ExtDirectConstants.CONTENT_TYPE_APPLICATION_JSON, ExtDirectConstants.CONTENT_TYPE_TEXT_JSON, ExtDirectConstants.CONTENT_TYPE_TEXT_PLAIN)]
        [Produces(ExtDirectConstants.CONTENT_TYPE_APPLICATION_JSON)]
        public async Task<IActionResult> OnAction(
            [FromRoute] string providerName,
            [FromBody][ModelBinder(typeof(ExtDirectRemotingRequestModelBinder))] RemotingRequestBatch request)
        {
            var handler = new ExtDirectActionHandler(_serviceProvider, providerName, request);
            var result = await handler.ExecuteAsync();
            return await Json(result);
        }

        /// <summary>
        /// Handles client Ext Direct polling requests
        /// </summary>
        /// <param name="providerName"></param>
        /// <see href="https://docs.sencha.com/extjs/7.0.0/guides/backend_connectors/direct/specification.html"/>
        [AcceptVerbs("GET")]
        [Produces(ExtDirectConstants.CONTENT_TYPE_APPLICATION_JSON)]
        public async Task<IActionResult> OnEvents([FromRoute] string providerName)
        {
            var handler = new ExtDirectPollingEventHandler(_serviceProvider, ControllerContext, providerName);
            var result = await handler.ExecuteAsync();
            return await Json(result);
        }

        /// <summary>
        /// Uses Newtonsoft.Json
        /// </summary>
        private new async Task<IActionResult> Json(object data)
        {
            var json = JsonConvert.SerializeObject(data, Utils.Util.DefaultSerializerSettings);
            var content = Content(json, ExtDirectConstants.CONTENT_TYPE_APPLICATION_JSON, Encoding.UTF8);
            return await Task.FromResult(content);
        }
    }
}