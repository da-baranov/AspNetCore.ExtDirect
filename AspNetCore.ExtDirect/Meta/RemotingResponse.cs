namespace AspNetCore.ExtDirect.Meta
{
    public class RemotingResponse
    {
        public RemotingResponse()
        {

        }

        public RemotingResponse(RemotingRequest request, object result = null)
        {
            this.Action = request.Action;
            this.Method = request.Method;
            this.Tid = request.Tid;
            this.Result = result;
        }

        /// <summary>
        /// MUST be a string "rpc"
        /// </summary>
        public string Type { get; } = "rpc";

        /// <summary>
        /// The transaction id for this Response. MUST be the same as in the original Request.
        /// </summary>
        public int Tid { get; set; }

        /// <summary>
        /// The Action to which the invoked Method belongs to. MUST be the same as in the original Request.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        ///  The name of the invoked Remoting Method. MUST be the same as in the original Request.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The data returned by the Method. MUST be present in the Response object, but MAY be null for Methods that do not return any data.
        /// </summary>
        public object Result { get; set; } = null;
    }
}
