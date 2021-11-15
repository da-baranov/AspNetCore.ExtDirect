using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ExtDirectMethodAttribute : Attribute
    {
        public string Name { get; private set; }

        public ExtDirectMethodAttribute(string name = null)
        {
            Name = name;
        }
    }
}
