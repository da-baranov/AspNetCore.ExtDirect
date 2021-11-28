using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;

namespace AspNetCore.ExtDirect.Utils
{
    /// <summary>
    /// Internal utility functions
    /// </summary>
    internal static class Util
    {
        private static readonly JsonSerializerSettings _defaultSerializerSettings;

        static Util()
        {
            // Default JSON serializer settings
            var camelCaseNamingStrategy = new CamelCaseNamingStrategy();
            _defaultSerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = camelCaseNamingStrategy
                }
            };
            _defaultSerializerSettings.Converters.Add(new StringEnumConverter
            {
                NamingStrategy = camelCaseNamingStrategy
            });
        }

        public static JsonSerializerSettings DefaultSerializerSettings => _defaultSerializerSettings;

        /// <summary>
        /// Serializes an object to JSON
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="value">An object being serialized</param>
        /// <returns>JSON string</returns>
        internal static string JsonSerialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value, _defaultSerializerSettings);
        }

        internal static string ToCamelCase(string value)
        {
            // return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value); - not working
            return System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(value);
        }

        internal static string Uuid()
        {
            return Guid.NewGuid().ToString().ToLowerInvariant();
        }
    }
}