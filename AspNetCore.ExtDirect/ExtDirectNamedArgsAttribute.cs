using System;

namespace AspNetCore.ExtDirect
{
    /// <summary>
    /// Tells a caller that this Method accepts named arguments
    /// </summary>
    /// <see href="https://docs.sencha.com/extjs/7.0.0/classic/Ext.direct.RemotingProvider.html"/>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ExtDirectNamedArgsAttribute : Attribute
    {
    }
}
