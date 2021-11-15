using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ExtDirectNamedArgsAttribute : Attribute
    {
    }
}
