using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect.Meta
{
    public sealed class RemotingApi
    {
        public RemotingApi()
        {

        }

        internal RemotingApi(ExtDirectRemotingApiOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            Id = options.Id;
            Name = options.Name;
            Namespace = options.Namespace;
            Timeout = options.Timeout;
        }

        internal string Name { get; set; }

        /// <summary>
        /// The identifier for the Remoting API Provider. This is useful when there are more than one API in use
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The Service URI for this API
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// MUST be either remoting for Remoting API, or polling for Polling API
        /// </summary>
        public RemotingType Type { get; set; } = RemotingType.Remoting;

        /// <summary>
        /// The Namespace for the given Remoting API
        /// </summary>
        public string Namespace { get; set; }

        public bool ShouldSerializeNamespace() => !string.IsNullOrWhiteSpace(Namespace);

        /// <summary>
        /// The number of milliseconds to use as the timeout for every Method invocation in this Remoting API
        /// </summary>
        public int? Timeout { get; set; }

        public bool ShouldSerializeTimeout() => Timeout != null;

        /// <summary>
        ///  Array of Objects that represent Methods
        /// </summary>
        public Dictionary<string, RemotingAction> Actions { get; private set; }
            = new Dictionary<string, RemotingAction>(StringComparer.InvariantCultureIgnoreCase);
    }
}
