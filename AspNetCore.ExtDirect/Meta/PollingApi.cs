using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.ExtDirect.Meta
{
    public sealed class PollingApi
    {
        public PollingApi()
        {

        }

        internal PollingApi(ExtDirectPollingEventHandlerOptions options)
        {
            Name = options.Name;
            Id = options.Id;
            HandlerTypes.AddRange(options.HandlerTypes.Where(row => row != null).Distinct());
        }

        internal string Name { get; set; }

        /// <summary>
        /// The identifier for the Polling API Provider. This is useful when there are more than one API in use.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///  MUST be "polling" for Polling API.
        /// </summary>
        public RemotingType Type { get; } = RemotingType.Polling;

        /// <summary>
        /// The Service URI for this API.
        /// </summary>
        public string Url { get; set; }


        internal List<Type> HandlerTypes { get; private set; } = new();
    }
}