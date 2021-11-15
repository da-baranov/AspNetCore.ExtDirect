using AspNetCore.ExtDirect.Meta;
using AspNetCore.ExtDirect.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AspNetCore.ExtDirect
{
    public class ExtDirectHandlerRepository
    {
        private IStringLocalizer _localizer;
        private IStringLocalizerFactory _localizerFactory;
        private IServiceProvider _services;
        internal ExtDirectHandlerRepository(IServiceProvider services)
        {
            _services = services
                ?? throw new ArgumentNullException(nameof(services));
            _localizerFactory = _services.GetService<IStringLocalizerFactory>();
            _localizer = _localizerFactory.Create(typeof(Properties.Resources));
        }

        internal List<Type> PollingHandlers { get; private set; } = new();
        internal Dictionary<string, ExtActionType> RemotingHandlers { get; private set; } = new();

        public void RegisterPollingHandler<T>()
            where T : IExtDirectPollingEventSource
        {
            if (!PollingHandlers.Contains(typeof(T)))
            {
                PollingHandlers.Add(typeof(T));
            }
        }

        /// <summary>
        /// Registers an ExtDirect action handler
        /// </summary>
        /// <param name="actionHandlerType">Type of ExtDirect action handler</param>
        /// <param name="actionName">Action name</param>
        public void RegisterRemotingHandler(Type actionHandlerType, string actionName = null)
        {
            if (actionHandlerType == null)
            {
                throw new ArgumentNullException(nameof(actionHandlerType));
            }

            var internalActionName = actionName;

            var extActionAttribute = actionHandlerType.GetCustomAttribute<ExtDirectActionAttribute>(false);
            if (extActionAttribute != null)
            {
                if (!string.IsNullOrEmpty(extActionAttribute.Name))
                {
                    internalActionName = extActionAttribute.Name;
                }
            }

            if (string.IsNullOrEmpty(internalActionName))
            {
                internalActionName = actionHandlerType.FullName;
            }

            if (RemotingHandlers.ContainsKey(internalActionName))
            {
                throw new Exception(_localizer[nameof(Properties.Resources.ERR_DUPLICATE_ACTION), internalActionName]);
            }
            else
            {
                RemotingHandlers.Add(internalActionName, new ExtActionType(internalActionName, actionHandlerType));
            }
        }

        public void ScanAssemblies()
        {
            var extHandlerTypes = Util.GetActionTypes();
            foreach (var extHandlerType in extHandlerTypes)
            {
                RegisterRemotingHandler(extHandlerType);
            }
        }

        internal void FindExtDirectActionAndMethod(string actionNameOrTypeName, string method, out Type type, out MethodInfo methodInfo)
        {
            var targetHadlerKey = this.RemotingHandlers.Keys.FirstOrDefault(row => string.Equals(row, actionNameOrTypeName, StringComparison.InvariantCultureIgnoreCase));
            if (string.IsNullOrEmpty(targetHadlerKey))
            {
                throw new Exception($"Cannot find ExtDirect action \"{actionNameOrTypeName}\" handler.");
            }

            // Looking for a Type
            var a = RemotingHandlers[targetHadlerKey];
            type = a.Type;

            // Looking for a MethodInfo
            var m = a.Methods.FirstOrDefault(row =>
                string.Equals(row.Name, method, StringComparison.InvariantCultureIgnoreCase));
            if (m == null)
            {
                throw new Exception($"Cannot find method \"{method}\" on ExtDirect action \"{actionNameOrTypeName}\" handler.");
            }
            methodInfo = m.Method;
        }

        internal RemotingApi ToRemotingApi()
        {
            var result = new RemotingApi();
            foreach (var key in RemotingHandlers.Keys)
            {
                var handler = RemotingHandlers[key];
                var extAction = new RemotingAction();
                foreach (var handlerMethod in handler.Methods)
                {
                    var extMethod = new RemotingMethod();
                    extMethod.Name = Util.ToCamelCase(handlerMethod.Name);
                    extMethod.Params.AddRange(handlerMethod.Args);
                    extMethod.Len = handlerMethod.Args.Count;
                    extMethod.NamedArguments = handlerMethod.NamedArguments;
                    extAction.Add(extMethod);
                }
                result.Actions.Add(key, extAction);
            }
            return result;
        }

        internal class ExtActionMethod
        {
            internal ExtActionMethod(MethodInfo methodInfo)
            {
                Method = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
                foreach (var param in Method.GetParameters())
                {
                    Args.Add(param.Name);
                }
            }

            internal bool NamedArguments { get; set; } = false;

            public List<string> Args { get; private set; } = new();

            public MethodInfo Method { get; private set; }

            public string Name => Method.Name;
        }

        internal class ExtActionType
        {
            internal ExtActionType(string name, Type type)
            {
                Type = type ?? throw new ArgumentNullException(nameof(type));

                Name = string.IsNullOrEmpty(name) ? type.FullName : name;

                var publicMethods =
                     Type
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .ToList();
                foreach (var publicMethod in publicMethods)
                {
                    var method = new ExtActionMethod(publicMethod);
                    var namedArgsAttribute = publicMethod.GetCustomAttribute<ExtDirectNamedArgsAttribute>();
                    if (namedArgsAttribute != null)
                    {
                        method.NamedArguments = true;
                    }
                    Methods.Add(method);
                }
            }

            internal List<ExtActionMethod> Methods { get; private set; } = new();

            internal string Name { get; set; }

            internal Type Type { get; set; }

            internal string TypeName => Type.FullName;
        }
    }
}