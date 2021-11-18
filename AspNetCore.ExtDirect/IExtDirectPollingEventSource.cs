using AspNetCore.ExtDirect.Meta;
using System.Collections.Generic;

namespace AspNetCore.ExtDirect
{
    /// <summary>
    /// Implementors should implement this interface to provide a client with a list of new data
    /// </summary>
    public interface IExtDirectPollingEventSource
    {
        IEnumerable<PollResponse> GetEvents(params object[] args);
    }
}