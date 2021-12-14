using AspNetCore.ExtDirect.Binders;
using AspNetCore.ExtDirect.Meta;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
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
        private readonly IStringLocalizerFactory _localizerFactory;
        private readonly IStringLocalizer _stringLocalizer;

        public ExtDirectApiController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _localizerFactory = serviceProvider.GetRequiredService<IStringLocalizerFactory>();
            _stringLocalizer = _localizerFactory.Create(typeof(Properties.Resources));
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
            var result = _repository.MakeJavaScriptApiDefinition(Url, _options);
            return await Task.FromResult(Content(result, ExtDirectConstants.CONTENT_TYPE_TEXT_JAVASCRIPT, Encoding.UTF8));
        }

        /// <summary>
        /// Handles client Ext Direct remoting requests
        /// </summary>
        /// <param name="providerId"></param>
        /// <param name="request"></param>
        /// <see href="https://docs.sencha.com/extjs/7.0.0/guides/backend_connectors/direct/specification.html"/>
        [AcceptVerbs("POST")]
        [Consumes(ExtDirectConstants.CONTENT_TYPE_APPLICATION_JSON, 
                  ExtDirectConstants.CONTENT_TYPE_TEXT_JSON, 
                  ExtDirectConstants.CONTENT_TYPE_TEXT_PLAIN,
                  ExtDirectConstants.CONTENT_TYPE_APPLICATION_X_WWW_FORM_URLENCODED,
                  ExtDirectConstants.CONTENT_TYPE_MULTIPART_FORM_DATA)
        ]
        public async Task<IActionResult> OnAction
        (
            [FromBody][ModelBinder(typeof(ExtDirectRemotingRequestModelBinder))] RemotingRequestBatch request,
            [FromRoute] string providerId = null
        )
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            if (string.IsNullOrWhiteSpace(providerId))
            {
                return new BadRequestObjectResult(_stringLocalizer[nameof(Properties.Resources.ERR_EMPTY_PROVIDER_ID)]);
            }

            // Handles both Remoting requests and Form requests
            var handler = new ExtDirectRemotingHandler(_serviceProvider, providerId);

            var result = await handler.ExecuteAsync(request);

            // Form POST request?
            if (result.Count == 1 && true == result[0].FormHandler)
            {
                if (result[0] is RemotingException remotingException)
                {
                    remotingException.Result = new { Success = false, Message = remotingException.Message };
                    return await Json(remotingException);
                }

                var remotingResponse = (RemotingResponse)result[0];
                dynamic tmp = remotingResponse.Result;
                remotingResponse.Result = new { Success = true, Result = tmp };

                // Form POST request has files?
                if (remotingResponse.HasFileUploads)
                {
                    var json = Utils.Util.JsonSerialize(remotingResponse);
                    var html = string.Format(@"<!DOCTYPE html>
                    <html>
                        <head>
                            <title>File upload response</title>
                        </head>
                        <body>
                            <textarea>{0}</textarea>
                        </body>
                    </html>", json);
                    return await Html(html);
                }

                // Form POST request without files
                return await Json(remotingResponse);
            }

            // Usual request
            return await Json(result);
        }

        /// <summary>
        /// Handles client Ext Direct polling requests
        /// </summary>
        /// <param name="providerId"></param>
        /// <see href="https://docs.sencha.com/extjs/7.0.0/guides/backend_connectors/direct/specification.html"/>
        [AcceptVerbs("GET")]
        [Produces(ExtDirectConstants.CONTENT_TYPE_APPLICATION_JSON)]
        public async Task<IActionResult> OnEvents([FromRoute] string providerId = null)
        {
            if (string.IsNullOrWhiteSpace(providerId))
            {
                return new BadRequestObjectResult(_stringLocalizer[nameof(Properties.Resources.ERR_EMPTY_PROVIDER_ID)]);
            }
            var handler = new ExtDirectPollingHandler(_serviceProvider, ControllerContext, providerId);
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

        private async Task<IActionResult> Html(string html)
        {
            var content = Content(html, ExtDirectConstants.CONTENT_TYPE_TEXT_HTML_UTF8);
            return await Task.FromResult(content);
        }
    }
}