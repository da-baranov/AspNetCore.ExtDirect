using System;

namespace AspNetCore.ExtDirect.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ExtDirectEventNameAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of this event
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventName">The name of the event</param>
        public ExtDirectEventNameAttribute(string eventName = null)
        {
            EventName = eventName;
        }
    }
}
