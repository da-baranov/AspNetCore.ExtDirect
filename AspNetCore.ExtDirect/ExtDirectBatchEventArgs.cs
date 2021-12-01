using System;

namespace AspNetCore.ExtDirect
{
    public sealed class ExtDirectBatchEventArgs : EventArgs
    {
        internal ExtDirectBatchEventArgs() { }

        internal ExtDirectBatchEventArgs(string batchId)
        {
            BatchId = batchId;
        }

        public Exception Exception { get; internal set; }

        public string BatchId { get; private set; }
    }
}
