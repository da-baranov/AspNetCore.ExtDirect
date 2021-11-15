using System.Collections.Generic;

namespace AspNetCore.ExtDirect.Meta
{
    public sealed class RemotingMethod
    {
        /// <summary>
        /// A JavaScript Boolean value of true indicates that this Method accepts Form submits
        /// </summary>
        public bool FormHandler { get; set; }

        /// <summary>
        /// The number of arguments required for Ordered Methods. This number MAY be 0 for Methods that do not accept arguments
        /// </summary>
        public int Len { get; set; } = 0;

        /// <summary>
        /// When present, Call Metadata arguments MUST conform to their calling convention. Call Metadata calling convention MAY be different from the main Method arguments calling convention, e.g. an Ordered Method MAY accept by-name Call Metadata, or Named Method MAY accept by-position Call Metadata.
        /// </summary>
        public RemotingMetadata Metadata { get; set; } = null;

        /// <summary>
        /// The name of this Method. MUST be unique within its Action.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Array of names of the supported for by-name Call Metadata. This Array MAY be empty.
        /// </summary>
        public List<string> Params { get; set; } = new();

        /// <summary>
        /// JavaScript Boolean value that controls how Call Metadata arguments are checked.
        /// </summary>
        public bool Strict { get; set; } = false;

        internal bool NamedArguments { get; set; }

        public bool ShouldSerializeLen() => !NamedArguments;

        public bool ShouldSerializeParams() => NamedArguments;
    }
}