using System;

namespace AspNetCore.ExtDirect
{
    /// <summary>
    /// Allows a developer to override Ext Direct remoting handler Action name
    /// </summary>
    /// <example>
    /// [ExtDirectAction("Demo")]
    /// public class DemoHandler
    /// {
    ///     public int Add(int a, int b) => a + b;
    /// }
    /// </example>
    [AttributeUsage(AttributeTargets.Class)]
    public class ExtDirectActionAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of this action handler
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The action name</param>
        public ExtDirectActionAttribute(string name = null)
        {
            Name = name;
        }
    }
}
