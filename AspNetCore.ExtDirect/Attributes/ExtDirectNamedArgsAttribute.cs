using System;

namespace AspNetCore.ExtDirect.Attributes
{
    /// <summary>
    /// Specifies a remote method with named arguments
    /// </summary>
    /// <see href="https://docs.sencha.com/extjs/7.0.0/classic/Ext.direct.RemotingProvider.html"/>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ExtDirectNamedArgsAttribute : Attribute
    {
    }
}
