using AspNetCore.ExtDirect.Meta;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect.Binders
{
    /// <summary>
    /// Converts Ext Direct JSON requests to an object
    /// </summary>
    internal sealed class ExtDirectRemotingRequestModelBinder : IModelBinder
    {
        // TODO: https://stackoverflow.com/questions/52350447/in-asp-net-core-how-can-i-get-the-multipart-form-data-from-the-body

        public ExtDirectRemotingRequestModelBinder()
        {
        }

        /// <summary>
        /// This method accepts either RemotingRequest or array of RemotingRequest, and performs some basic validation of input data
        /// </summary>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var services = bindingContext.HttpContext.RequestServices;
            var localizerFactory = services.GetService<IStringLocalizerFactory>();
            var localizer = localizerFactory.Create(typeof(Properties.Resources));

            var contentType = new ContentType(bindingContext.HttpContext.Request.ContentType);
            if 
            (
                !string.Equals(contentType.MediaType, ExtDirectConstants.CONTENT_TYPE_APPLICATION_JSON, StringComparison.InvariantCultureIgnoreCase) &&
                !string.Equals(contentType.MediaType, ExtDirectConstants.CONTENT_TYPE_TEXT_JSON, StringComparison.InvariantCultureIgnoreCase) &&
                !string.Equals(contentType.MediaType, ExtDirectConstants.CONTENT_TYPE_TEXT_PLAIN, StringComparison.InvariantCultureIgnoreCase)
            )
            {
                bindingContext.Result = ModelBindingResult.Failed();
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName,
                                                                   localizer[nameof(Properties.Resources.ERR_INVALID_EXT_REQUEST)]);
                return;
            }

            var charset = contentType.CharSet;
            var encoding = string.IsNullOrEmpty(charset) 
                ? Encoding.UTF8 
                : Encoding.GetEncoding(charset);

            var json = default(string);
            var batch = default(RemotingRequestBatch);

            using (var reader = new StreamReader(bindingContext.HttpContext.Request.Body, encoding))
            {
                json = await reader.ReadToEndAsync();
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                // Valid content-type but empty body? Consider this is OK
                batch = new RemotingRequestBatch();
            }
            else
            {
                try
                {
                    var token = JToken.Parse(json);
                    if (token is JArray jarray)
                    {
                        batch = jarray.ToObject<RemotingRequestBatch>();
                    }
                    else if (token is JObject jobject)
                    {
                        batch = new RemotingRequestBatch();
                        var request = jobject.ToObject<RemotingRequest>();
                        batch.Add(request);
                    }
                    else
                    {
                        bindingContext.Result = ModelBindingResult.Failed();
                        bindingContext.ModelState.TryAddModelError(bindingContext.ModelName,
                                                                   localizer[nameof(Properties.Resources.ERR_INVALID_EXT_REQUEST)]);
                    }
                }
                catch (Exception ex)
                {
                    bindingContext.ModelState.TryAddModelException(bindingContext.ModelName, ex);
                    bindingContext.Result = ModelBindingResult.Failed();
                }
            }

            bindingContext.Result = ModelBindingResult.Success(batch);
        }
    }
}