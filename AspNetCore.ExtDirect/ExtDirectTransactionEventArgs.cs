using System;

namespace AspNetCore.ExtDirect
{
    public sealed class ExtDirectTransactionEventArgs : EventArgs
    {
        public ExtDirectTransactionEventArgs() { }

        public ExtDirectTransactionEventArgs(string transactionId)
        {
            this.TransactionId = transactionId;
        }

        public Exception Exception { get; internal set; }

        public string TransactionId { get; private set; }
    }
}
