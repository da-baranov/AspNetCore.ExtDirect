namespace AspNetCore.ExtDirect.Meta
{
    public sealed class PollingApi
    {
        /// <summary>
        /// The identifier for the Polling API Provider. This is useful when there are more than one API in use.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///  MUST be "polling" for Polling API.
        /// </summary>
        public string Type { get; } = "polling";

        /// <summary>
        /// The Service URI for this API.
        /// </summary>
        public string Url { get; set; }
    }
}