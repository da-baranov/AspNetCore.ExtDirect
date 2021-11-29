using AspNetCore.ExtDirect.Meta;
using AspNetCore.ExtDirect.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect.Test
{
    internal static class IntegrationTestExtensions
    {
        public static async Task<T> CallNamed<T>(this HttpClient client, string action, string method, object data)
        {
            var request = new RemotingRequest
            {
                Action = action,
                Method = method,
                Tid = 1,
                Data = data
            };
            return await PostAsync<T>(client, request);
        }

        public static async Task<T> CallOrdered<T>(this HttpClient client, string action, string method, params object[] args)
        {
            var request = new RemotingRequest
            {
                Action = action,
                Method = method,
                Tid = 1,
                Data = args
            };
            return await PostAsync<T>(client, request);
        }

        public static async Task<List<PollResponse>> CallPolling(this HttpClient client)
        {
            var response = await client.GetAsync("/" + new ExtDirectOptions().PollingEndpointUrl + "/POLLING_TEST_API?FirstName=John&LastName=Doe");
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<PollResponse>>(responseString, Utils.Util.DefaultSerializerSettings);
            return result;
        }

        private static async Task<T> PostAsync<T>(this HttpClient client, object request)
        {
            var json = JsonConvert.SerializeObject(request, Utils.Util.DefaultSerializerSettings);
            var response = await client.PostAsync(
                                    "/" + new ExtDirectOptions().RemotingEndpointUrl + "/REMOTING_API", 
                                    new StringContent(json, Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("HTTP request failed.");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var serializerSettings = Utils.Util.CreateDefaultSerializerSettings();
            // serializerSettings.Converters.Clear();
            serializerSettings.Converters.Add(new ExtDirectResponseJsonConverter());

            var responseList = JsonConvert.DeserializeObject<List<RemotingResponseBase>>(responseString, serializerSettings);
            if (responseList.Count == 0)
            {
                throw new Exception("Response array is empty");
            }


            if (responseList[0] is RemotingException responseEx)
            {
                throw new Exception(responseEx.Message);
            }
            else if (responseList[0] is RemotingResponse responseItem)
            {
                if (responseItem.Result is null)
                {
                    return default;
                }

                else if (responseItem.Result is JToken jtoken)
                {
                    return jtoken.ToObject<T>();
                }

                else
                {
                    return (T)responseItem.Result;
                }
            }
            else
            {
                throw new Exception("Undefined call result");
            }
        }
    }
}
