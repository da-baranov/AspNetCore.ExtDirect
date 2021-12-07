using AspNetCore.ExtDirect.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AspNetCore.ExtDirect.Meta
{
    /// <summary>
    /// Array of Objects that represent Methods
    /// </summary>
    internal sealed class RemotingAction : List<RemotingMethod>
    {
        internal RemotingAction()
        {
        }

        internal RemotingAction(Type type)
        {
            ActionType = type ?? throw new ArgumentNullException(nameof(type));

            Name = type.Name;
            var attr = type.GetCustomAttribute<ExtDirectActionAttribute>();
            if (attr != null && !string.IsNullOrWhiteSpace(attr.Name))
            {
                Name = attr.Name;
            }

            // Collect public methods only
            var publicMethods = ActionType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            foreach (var publicMethod in publicMethods)
            {
                // Exclude methods marked with the ExtDirectIgnoreAttribute
                var extDirectIgnoreAttribute = publicMethod.GetCustomAttribute<ExtDirectIgnoreAttribute>();
                if (extDirectIgnoreAttribute == null)
                {
                    var remotingMethod = new RemotingMethod(publicMethod);
                    Add(remotingMethod);
                }
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
                if (string.IsNullOrWhiteSpace(name)) return null;

                return this.FirstOrDefault(row => string.Equals(row.Name, name, StringComparison.InvariantCultureIgnoreCase));
            }
        }
    }
}