using System;

namespace AspNetCore.ExtDirect
{
    public interface IExtDirectBatchService
    {
        public event EventHandler<ExtDirectBatchEventArgs> BatchBegin;
        public event EventHandler<ExtDirectBatchEventArgs> BatchCommit;
        public event EventHandler<ExtDirectBatchEventArgs> BatchException;
    }
}
