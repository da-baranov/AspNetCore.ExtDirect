using System;

namespace AspNetCore.ExtDirect
{
    internal sealed class ExtDirectBatchService : IExtDirectBatchService
    {
        public event EventHandler<ExtDirectBatchEventArgs> BatchBegin;

        public event EventHandler<ExtDirectBatchEventArgs> BatchCommit;

        public event EventHandler<ExtDirectBatchEventArgs> BatchException;

        internal void FireBatchBegin(string batchId)
        {
            BatchBegin?.Invoke(this, new ExtDirectBatchEventArgs(batchId));
        }

        internal void FireBatchCommit(string batchId)
        {
            BatchCommit?.Invoke(this, new ExtDirectBatchEventArgs(batchId));
        }

        internal void FireBatchRollback(string batchId, Exception ex)
        {
            BatchException?.Invoke(this, new ExtDirectBatchEventArgs(batchId) { Exception = ex });
        }
    }
}