using AspNetCore.ExtDirect.Meta;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace AspNetCore.ExtDirect.Utils
{
    /// <summary>
    /// For testing purposes only
    /// </summary>
    internal sealed class ExtDirectResponseJsonConverter : JsonConverter<RemotingResponseBase>
    {
        public override RemotingResponseBase ReadJson(JsonReader reader, Type objectType, RemotingResponseBase existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            if (objectType == typeof(RemotingResponseBase))
            {
                JObject jsonObject = JObject.Load(reader);

                if (jsonObject.Property("type")?.Value is JValue jv && jv.Value is string type)
                {
                    if (type == "rpc")
                    {
                        var r = jsonObject.ToObject<RemotingResponse>();
                        return r;
                    }
                    else if (type == "exception")
                    {
                        var ex = jsonObject.ToObject<RemotingException>();
                        return ex;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, RemotingResponseBase value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}