using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect.Meta
{
    /// <summary>
    /// An Ext Direct exception
    /// </summary>
    public sealed class RemotingException
    {
        public RemotingException()
        {

        }

        public RemotingException(RemotingRequest request, Exception ex)
        {
            this.Message = ex.Message;
            this.Where = ex.StackTrace;
            this.Tid = request.Tid;
            this.Action = request.Action;
            this.Method = request.Method;
        }

        public RemotingException(Exception ex, string action, string method, int tid)
        {
            this.Message = ex.Message;
            this.Where = ex.StackTrace;
            this.Tid = tid;
            this.Action = action;
            this.Method = method;
            this.Where = ex.StackTrace;
        }

        /// <summary>
        /// The error message. MUST be present.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// MUST be a string "exception"
        /// </summary>
        public string Type { get; } = "exception";

        /// <summary>
        /// The transaction id for this Response. MUST be the same as in the original Request.
        /// </summary>
        public int Tid { get; set; }

        /// <summary>
        /// The Action to which the invoked Method belongs to. MUST be the same as in the original Request.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// The name of the invoked Remoting Method. MUST be the same as in the original Request.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// OPTIONAL description of where exactly the exception was raised. MAY contain stack trace, or additional information.
        /// </summary>
        public string Where { get; set; }
    }
}
