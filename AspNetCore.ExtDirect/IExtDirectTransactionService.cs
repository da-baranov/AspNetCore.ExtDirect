using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect
{
    public interface IExtDirectTransactionService
    {
        public event EventHandler<ExtDirectTransactionEventArgs> TransactionBegin;
        public event EventHandler<ExtDirectTransactionEventArgs> TransactionCommit;
        public event EventHandler<ExtDirectTransactionEventArgs> TransactionRollback;
    }
}
