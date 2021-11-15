using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect
{
    public class ExtDirectOptions
    {
        public ExtDirectOptions()
        {
        }

        public string RemotingApiId { get; set; } = "AspNetCore.ExtDirect";

        public string RemotingRouteUrl { get; set; } = "ExtDirect";

        public string RemotingApiName { get; set; } = "REMOTING_API";

        public string PollingApiId { get; set; } = "AspNetCore.ExtDirectEvents";

        public string PollingRouteUrl { get; set; } = "ExtDirectEvents";

        public string PollingApiName { get; set; } = "POLLING_API";
    }
}
