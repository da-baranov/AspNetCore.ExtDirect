using AspNetCore.ExtDirect.Meta;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect.Test
{
    public static class IntegrationTestExtensions
    {
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

        public static async Task<T> CallNamed<T>(this HttpClient client, string action, string method,object data)
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

        public static async Task<List<PollResponse>> CallPolling(this HttpClient client)
        {
            var response = await client.GetAsync("/" + new ExtDirectOptions().PollingRouteUrl + "/POLLING_API");
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<PollResponse>>(responseString, Utils.Util.DefaultSerializerSettings);
            return result;
        }

        private static async Task<T> PostAsync<T>(this HttpClient client, object request)
        {
            var json = JsonConvert.SerializeObject(request, Utils.Util.DefaultSerializerSettings);
            var response = await client.PostAsync("/" + new ExtDirectOptions().RemotingRouteUrl + "/REMOTING_API", new StringContent(json));
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("HTTP request failed.");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var responseList = JsonConvert.DeserializeObject<List<RemotingResponse>>(responseString, Utils.Util.DefaultSerializerSettings);
            if (responseList.Count == 0)
            {
                throw new Exception("Response array is empty");
            }
            var responseItem = responseList[0];
            if (responseItem.Result == null)
            {
                return default;
            }
            if (responseItem.Result is JToken jtoken)
            {
                return jtoken.ToObject<T>();
            }
            else
            {
                return (T)responseItem.Result;
            }
        }
    }
}
