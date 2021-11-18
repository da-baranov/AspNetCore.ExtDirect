using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AspNetCore.ExtDirect.Meta
{
    /// <summary>
    /// Array of Objects that represent Methods
    /// </summary>
    public sealed class RemotingAction : List<RemotingMethod>
    {
        public RemotingAction()
        {
        }

        public RemotingAction(Type type)
        {
            ActionType = type ?? throw new ArgumentNullException(nameof(type));

            Name = type.Name;
            var attr = type.GetCustomAttribute<ExtDirectActionAttribute>();
            if (attr != null && !string.IsNullOrWhiteSpace(attr.Name))
            {
                Name = attr.Name;
            }

            var publicMethods = ActionType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            foreach (var publicMethod in publicMethods)
            {
                var remotingMethod = new RemotingMethod(publicMethod);
                this.Add(remotingMethod);
            }
        }

        internal Type ActionType { get; private set; }
        /// <summary>
        /// Internal action name
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// Locates a method
        /// </summary>
        /// <param name="name">Name of method being searched for</param>
        /// <returns></returns>
        public RemotingMethod this[string name]
        {
            get
            {
                return this.FirstOrDefault(row => string.Equals(row.Name, name, StringComparison.InvariantCultureIgnoreCase));
            }
        }
    }
}