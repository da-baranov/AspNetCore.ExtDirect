using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.ExtDirect.Meta
{
    internal sealed class PollingApi : ApiBase
    {
        public PollingApi()
        {
        }

        internal PollingApi(ExtDirectPollingApiOptions options)
        {
            Name = options.Name;
            Id = options.Id;
            HandlerTypes.AddRange(options.Handlers.Distinct());
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
        internal List<ExtDirectPollingHandlerRegistryItem> HandlerTypes { get; private set; } = new();
    }
}