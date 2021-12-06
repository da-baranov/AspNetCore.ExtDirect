using System;
using System.Reflection;

namespace AspNetCore.ExtDirect
{
    internal sealed record ExtDirectPollingHandlerRegistryItem
    {
        internal Type HandlerType { get; set; }
        internal object Func { get; set; }
        internal MethodInfo Delegate { get; set; }
        internal bool IsSync { get; set; }
        internal Type ParameterType { get; set; }
        internal string EventName { get; set; }

        public override int GetHashCode()
        {
            var result = HandlerType.GetHashCode();
            result ^= Delegate.GetHashCode();
            result ^= IsSync.GetHashCode();
            if (ParameterType != null)
            {
                result ^= ParameterType.GetHashCode();
            }
            if (!string.IsNullOrWhiteSpace(EventName))
            {
                result ^= EventName.GetHashCode();
            }
            return result;
        }
    }
}