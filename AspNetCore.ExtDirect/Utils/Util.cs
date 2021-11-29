using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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
            _defaultSerializerSettings = CreateDefaultSerializerSettings();
        }

        public static JsonSerializerSettings CreateDefaultSerializerSettings()
        {
            // Default JSON serializer settings
            var camelCaseNamingStrategy = new CamelCaseNamingStrategy();
            var defaultSerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = camelCaseNamingStrategy
                }
            };
            defaultSerializerSettings.Converters.Add(new StringEnumConverter
            {
                NamingStrategy = camelCaseNamingStrategy
            });
            return defaultSerializerSettings;
        }

        public static JsonSerializerSettings DefaultSerializerSettings => _defaultSerializerSettings;

        /// <summary>
        /// Serializes an object to JSON using Newtonsoft converter
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

        internal static bool IsMethodAsync(MethodInfo methodInfo)
        {
            if (methodInfo.GetCustomAttribute<AsyncStateMachineAttribute>(false) != null)
            {
                return true;
            }
            if ((methodInfo.ReturnType != null)
                &&
                (
                    methodInfo.ReturnType.IsSubclassOf(typeof(Task<>)) ||
                    methodInfo.ReturnType.IsSubclassOf(typeof(Task))
                ))
            {
                return true;
            }
            if (methodInfo.Name.EndsWith("async", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }

        internal async static Task<object> InvokeSyncOrAsync(object sender, MethodInfo methodInfo, params object[] args)
        {
            if (IsMethodAsync(methodInfo))
            {
                var task = (Task)methodInfo.Invoke(sender, args);
                await task.ConfigureAwait(false);
                var resultProperty = task.GetType().GetProperty("Result");

                if (resultProperty == null) return null;

                var rv = resultProperty.GetValue(task);
                return rv;
            }
            else
            {
                var result = methodInfo.Invoke(sender, args);
                return await Task.FromResult(result);
            }
        }
    }
}