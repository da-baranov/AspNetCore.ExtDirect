﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExtDirectActionAttribute : Attribute
    {
        public string Name { get; private set; }

        public ExtDirectActionAttribute(string name = null)
        {
            Name = name;
        }
    }
}
