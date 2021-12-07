using System.Collections.Generic;

namespace AspNetCore.ExtDirect.Meta
{
    public sealed class RemotingMetadata
    {
        /// <summary>
        /// The number of arguments required for by-position Call Metadata. This number MUST be greater than 0.
        /// </summary>
        public int Len { get; set; }

        /// <summary>
        /// The Array of names of the supported for by-name Call Metadata. This Array MAY be empty.
        /// </summary>
        public List<string> Params { get; private set; } = new();

        /// <summary>
        ///  JavaScript Boolean value that controls how Call Metadata arguments are checked.
        /// </summary>
        public bool Strict { get; set; }
    }
}