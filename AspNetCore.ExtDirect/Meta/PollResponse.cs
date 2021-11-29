namespace AspNetCore.ExtDirect.Meta
{
    public class PollResponse : ResponseBase
    {
        /// <summary>
        /// MUST be a string "event"
        /// </summary>
        public override string Type { get; } = "event";

        /// <summary>
        /// Event name, MUST be a string.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Event data, SHOULD be any valid JSON data.
        /// </summary>
        public object Data { get; set; }
    }
}
