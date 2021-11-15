using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace AspNetCore.ExtDirect.Utils
{
    /// <summary>
    /// Internal utility functions
    /// </summary>
    internal static class Util
    {
        private static List<Type> _actionTypes = null;
        private static List<Type> _allTypes = null;
        private static JsonSerializerSettings _defaultSerializerSettings;
        private static List<Type> _nonAbstractClassTypes = null;

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

        internal static string ToCamelCase(string value)
        {
            // return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value); - not working
            return System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(value);
        }

        /// <summary>
        /// Cached list of all public, non-generic and non-abstract types having ExtActionAttribute set
        /// </summary>
        /// <returns></returns>
        internal static List<Type> GetActionTypes()
        {
            if (_actionTypes == null)
            {
                _actionTypes = new List<Type>();
                var publicTypes = GetNonAbstractClassTypes();
                _actionTypes.AddRange(publicTypes.Where(type => type.GetCustomAttribute<ExtDirectActionAttribute>(false) != null));
            }
            return _actionTypes;
        }

        /// <summary>
        /// Cached list of all types within current AppDomain
        /// </summary>
        /// <returns></returns>
        internal static List<Type> GetAllTypes()
        {
            if (_allTypes == null)
            {
                _allTypes = new List<Type>();
                var t = AppDomain
                    .CurrentDomain
                    .GetAssemblies()
                    .AsParallel()
                    .Select(row => row.GetTypes().AsParallel())
                    .ToList();
                foreach (var t1 in t)
                {
                    _allTypes.AddRange(t1);
                }
            }
            return _allTypes;
        }

        /// <summary>
        /// Cached list of all public, non-abstract and non-generic types within current AppDomain
        /// </summary>
        /// <returns></returns>
        internal static List<Type> GetNonAbstractClassTypes()
        {
            if (_nonAbstractClassTypes == null)
            {
                _nonAbstractClassTypes = new List<Type>();
                var t = AppDomain
                    .CurrentDomain
                    .GetAssemblies()
                    .AsParallel()
                    .Select(row => row.GetTypes().AsParallel().Where(row => row.IsPublic &&
                                                                           !row.IsGenericType &&
                                                                           !row.IsAbstract &&
                                                                            row.IsClass))
                    .ToList();
                foreach (var t1 in t)
                {
                    _nonAbstractClassTypes.AddRange(t1);
                }
            }
            return _nonAbstractClassTypes;
        }

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
    }
}