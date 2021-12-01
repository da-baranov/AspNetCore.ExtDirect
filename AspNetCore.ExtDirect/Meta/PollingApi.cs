using System;
using System.Collections.Generic;

namespace AspNetCore.ExtDirect.Meta
{
    public sealed class PollingApi : ApiBase
    {
        public PollingApi()
        {
        }

        internal PollingApi(ExtDirectPollingApiOptions options)
        {
            Name = options.Name;
            Id = options.Id;
            foreach (var key in options.HandlerTypes.Keys)
            {
                if (!HandlerTypes.ContainsKey(key))
                {
                    HandlerTypes.Add(key, options.HandlerTypes[key]);
                }
                else
                {
                    HandlerTypes[key] = options.HandlerTypes[key];
                }
            }
        }

        /// <summary>
        /// How often to poll the server-side in milliseconds. Defaults to every 3 seconds.
        /// </summary>
        public int? Interval { get; set; } = 3000;

        /// <summary>
        ///  MUST be "polling" for Polling API.
        /// </summary>
        public override RemotingType Type => RemotingType.Polling;

        /// <summary>
        /// The Service URI for this API.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// All the registered polling handler types
        /// </summary>
        internal Dictionary<Type, object> HandlerTypes { get; private set; } = new();
    }
}