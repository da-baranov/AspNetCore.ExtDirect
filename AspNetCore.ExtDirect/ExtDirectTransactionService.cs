using System;

namespace AspNetCore.ExtDirect
{
    internal class ExtDirectTransactionService : IExtDirectTransactionService
    {
        public event EventHandler<ExtDirectTransactionEventArgs> TransactionBegin;

        public event EventHandler<ExtDirectTransactionEventArgs> TransactionCommit;

        public event EventHandler<ExtDirectTransactionEventArgs> TransactionRollback;

        internal void FireTransactionBegin(string transactionId)
        {
            TransactionBegin?.Invoke(this, new ExtDirectTransactionEventArgs(transactionId));
        }

        internal void FireTransactionCommit(string transactionId)
        {
            TransactionCommit?.Invoke(this, new ExtDirectTransactionEventArgs(transactionId));
        }

        internal void FireTransactionRollback(string transactionId, Exception ex)
        {
            TransactionRollback?.Invoke(this, new ExtDirectTransactionEventArgs(transactionId) { Exception = ex });
        }
    }
}