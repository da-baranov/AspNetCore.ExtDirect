namespace AspNetCore.ExtDirect.Meta
{
    internal abstract class ApiBase
    {
        /// <summary>
        /// The identifier for the API Provider. This is useful when there are more than one API in use. Should be unique within a web application
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// The name for the API Provider. Should be unique within a web application
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// MUST be either remoting for Remoting API, or polling for Polling API
        /// </summary>
        public abstract RemotingType Type { get; }

    }
}
