using AspNetCore.ExtDirect.Meta;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.ExtDirect.Demo
{
    public class ChatHandler : IExtDirectPollingEventSource
    {
        private static int Id = 0;

        private static List<ChatMessage> Messages { get; } = new();

        public IEnumerable<PollResponse> GetEvents()
        {
            foreach (var message in Messages.Where(row => row.Read == false))
            {
                message.Read = true;
                yield return new PollResponse { Name = "onmessage", Data = message };
            }
        }

        public void SendMessage(string message)
        {
            Messages.Add(new ChatMessage
            {
                Id = ++Id,
                Date = DateTime.Now,
                Message = message,
                Read = false
            });
        }
    }

    public class ChatMessage
    {
        public DateTime Date { get; set; }

        public int Id { get; set; }

        public string Message { get; set; }

        internal bool Read { get; set; } = false;
    }
}