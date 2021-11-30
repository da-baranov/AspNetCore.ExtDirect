using System;

namespace AspNetCore.ExtDirect
{
    public sealed class ExtDirectBatchEventArgs : EventArgs
    {
        public ExtDirectBatchEventArgs() { }

        public ExtDirectBatchEventArgs(string transactionId)
        {
            this.TransactionId = transactionId;
        }

        public Exception Exception { get; internal set; }

        public string TransactionId { get; private set; }
    }
}
