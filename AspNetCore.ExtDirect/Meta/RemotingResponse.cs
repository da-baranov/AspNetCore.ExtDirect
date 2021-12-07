namespace AspNetCore.ExtDirect.Meta
{
    internal sealed class RemotingResponse : RemotingResponseBase
    {
        internal RemotingResponse()
        {
            this.Success = true;
        }

        internal RemotingResponse(RemotingRequest request, object result = null)
        {
            this.Action = request.Action;
            this.Method = request.Method;
            this.Tid = request.Tid;
            this.FormHandler = request.FormHandler;
            this.Result = result;
            this.Success = true;
            this.HasFileUploads = request.FormFiles != null && request.FormFiles.Count > 0;
        }

        /// <summary>
        /// MUST be a string "rpc"
        /// </summary>
        public override string Type { get; } = "rpc";

        /// <summary>
        /// The data returned by the Method. MUST be present in the Response object, but MAY be null for Methods that do not return any data.
        /// </summary>
        public object Result { get; set; } = null;
    }
}
