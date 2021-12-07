using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace AspNetCore.ExtDirect.Meta
{
    /// <summary>
    /// ExtDirect Request
    /// </summary>
    public sealed class RemotingRequest
    {
        /// <summary>
        /// MUST be a string "rpc".
        /// </summary>
        public string Type { get; } = "rpc";

        /// <summary>
        /// The transaction id for this Request. MUST be an integer number unique among the Requests in one batch.
        /// </summary>
        public int Tid { get; set; }

        /// <summary>
        /// The Action that the invoked Method belongs to. MUST be specified.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// The Remoting Method that is to be invoked. MUST be specified.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        ///  A set of arguments to be passed to the called Remoting Method. MUST be either null for Methods that accept 0 (zero) parameters, an Array for Ordered methods, or an Object for Named methods.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Stringified Boolean value ("true" or "false") indicating that file uploads are attached to this form submission (for forms)
        /// </summary>
        public bool Upload { get; set; }

        /// <summary>
        /// OPTIONAL set of meta-arguments to be made available to the called Remoting Method, if provided by the Client. If no metadata is associated with the call, this member MUST NOT be present in the Request.
        /// </summary>
        public List<RemotingMetadata> Metadata { get; } = new();

        /// <summary>
        /// Uploaded files
        /// </summary>
        public List<IFormFile> FormFiles { get; } = new();

        internal bool FormHandler { get; set; }
    }

    public sealed class RemotingRequestBatch : List<RemotingRequest>
    {
    }
}