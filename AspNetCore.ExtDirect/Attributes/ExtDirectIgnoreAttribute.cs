using System;

namespace AspNetCore.ExtDirect.Attributes
{
    /// <summary>
    /// Makes a public method of remoting handler unavailable to discover and call
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ExtDirectIgnoreAttribute : Attribute
    {

    }
}
