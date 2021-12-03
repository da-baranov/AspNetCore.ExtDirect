using System;

namespace AspNetCore.ExtDirect.Attributes
{
    /// <summary>
    /// The server-side must mark the submit handler as a 'formHandler'
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ExtDirectFormSubmitAttribute : Attribute
    {

    }
}
