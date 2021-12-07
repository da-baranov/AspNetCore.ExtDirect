using AspNetCore.ExtDirect.Meta;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect.Binders
{
    /// <summary>
    /// Converts an Ext Direct JSON request to an array of RemotingRequest (batch)
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

            // Usial JSON POST request
            if
            (
                string.Equals(contentType.MediaType, ExtDirectConstants.CONTENT_TYPE_APPLICATION_JSON, StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(contentType.MediaType, ExtDirectConstants.CONTENT_TYPE_TEXT_JSON, StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(contentType.MediaType, ExtDirectConstants.CONTENT_TYPE_TEXT_PLAIN, StringComparison.InvariantCultureIgnoreCase)
            )
            {
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
                    bindingContext.Result = ModelBindingResult.Success(batch);
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
                        bindingContext.Result = ModelBindingResult.Success(batch);
                    }
                    catch (Exception ex)
                    {
                        // TODO: throw?
                        bindingContext.ModelState.TryAddModelException(bindingContext.ModelName, ex);
                        bindingContext.Result = ModelBindingResult.Failed();
                    }
                }
            }

            // Form POST
            else if (
                string.Equals(contentType.MediaType, ExtDirectConstants.CONTENT_TYPE_APPLICATION_X_WWW_FORM_URLENCODED, StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(contentType.MediaType, ExtDirectConstants.CONTENT_TYPE_MULTIPART_FORM_DATA, StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    var request = new RemotingRequest
                    {
                        // Set form handler flag
                        FormHandler = true,

                        // Action, method, tid
                        Action = bindingContext.ValueProvider.GetValue("extAction").FirstValue,
                        Method = bindingContext.ValueProvider.GetValue("extMethod").FirstValue,
                        Tid = Convert.ToInt32(bindingContext.ValueProvider.GetValue("extTID").FirstValue)
                    };
                    var formUpload = bindingContext.ValueProvider.GetValue("extUpload").FirstValue;

                    // Form upload?
                    if (!string.IsNullOrWhiteSpace(formUpload))
                    {
                        request.Upload = (formUpload == "true");
                    }

                    // Files
                    var formFiles = bindingContext.HttpContext?.Request?.Form?.Files;
                    if (formFiles != null)
                    {
                        request.FormFiles.AddRange(formFiles);
                    }

                    // Other arguments
                    var form = bindingContext.HttpContext?.Request?.Form;
                    if (form != null)
                    {
                        var dictionary = new Dictionary<string, string>();
                        foreach (var key in form.Keys)
                        {
                            var value = (string)form[key];
                            dictionary.TryAdd(key, value);
                        }
                        var json = JsonConvert.SerializeObject(dictionary, Utils.Util.DefaultSerializerSettings);
                        var jtoken = JToken.Parse(json);
                        request.Data = jtoken;
                    }

                    // Prepare batch
                    var batch = new RemotingRequestBatch
                    {
                        request
                    };
                    bindingContext.Result = ModelBindingResult.Success(batch);
                }
                catch (Exception ex)
                {
                    // TODO: throw?
                    bindingContext.ModelState.TryAddModelException(bindingContext.ModelName, ex);
                    bindingContext.Result = ModelBindingResult.Failed();
                }
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName,
                                                                   localizer[nameof(Properties.Resources.ERR_INVALID_EXT_REQUEST)]);
                return;
            }
        }
    }
}