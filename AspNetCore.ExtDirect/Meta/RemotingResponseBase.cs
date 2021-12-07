namespace AspNetCore.ExtDirect.Meta
{
    internal abstract class RemotingResponseBase : ResponseBase
    {
        /// <summary>
        /// The Action to which the invoked Method belongs to. MUST be the same as in the original Request.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        ///  The name of the invoked Remoting Method. MUST be the same as in the original Request.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The transaction id for this Response. MUST be the same as in the original Request.
        /// </summary>
        public int Tid { get; set; }

        /// <summary>
        /// Not documented properly. Required for Ext Direct form POST responses.
        /// </summary>
        public bool Success { get; set; }

        internal bool FormHandler { get; set; }

        internal bool HasFileUploads { get; set; }
    }
}
