using AspNetCore.ExtDirect.Attributes;
using FluentValidation;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect
{
    public class ExtDirectPollingApiOptions
    {
        private readonly List<ExtDirectPollingHandlerRegistryItem> _handlers = new();

        

        private string _id;

        /// <summary>
        /// An identifier of the API is being registered. By default this is a random UUID
        /// </summary>
        public string Id
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_id)) return _id;
                return Name;
            }
            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// How often to poll the server-side in milliseconds. Defaults to every 3 seconds.
        /// </summary>
        public int? Interval { get; set; } = 3000;

        /// <summary>
        /// Gets or sets the name of the current API. This name should be unique within a web application.
        /// </summary>
        public string Name { get; set; } = "POLLING_API";

        /// <summary>
        /// List of registered handlers
        /// </summary>
        internal IReadOnlyList<ExtDirectPollingHandlerRegistryItem> Handlers => _handlers;

        /// <summary>
        /// Registers a synchronous polling event handler that receives arguments from query string
        /// </summary>
        /// <typeparam name="T">Type of class that implements this handler</typeparam>
        /// <typeparam name="T1">Type of argument</typeparam>
        /// <param name="func">A Func<T, IEnumerable> to process </param>
        /// <param name="eventName">The name of the event. If not set, default value "ondata" will be used.</param>
        public void AddHandler<T, T1>(Func<T, IEnumerable> func, string eventName = null)
            where T : class
            where T1 : class
        {
            AddHandler(typeof(T), func, func.Method, typeof(T1), eventName, true);
        }

        /// <summary>
        /// Registers a synchronous polling event handler that receives no arguments
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="eventName">The name of the event. If not set, default value "ondata" will be used.</param>
        public void AddHandler<T>(Func<T, IEnumerable> func, string eventName = null)
            where T : class
        {
            AddHandler(typeof(T), func, func.Method, null, eventName, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="eventName"></param>
        public void AddHandler<T>(Func<T, Task<IEnumerable>> func, string eventName = null)
        {
            AddHandler(typeof(T), func, func.Method, null, eventName, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="func"></param>
        /// <param name="eventName"></param>
        public void AddHandler<T, T1>(Func<T, T1, Task<IEnumerable>> func, string eventName = null)
        {
            AddHandler(typeof(T), func, func.Method, typeof(T1), eventName, true);
        }

        private void AddHandler([DisallowNull] Type handlerType, 
                                [DisallowNull] object func, 
                                [DisallowNull] MethodInfo methodInfo, 
                                [MaybeNull] Type parameterType, 
                                [MaybeNull] string eventName, 
                                bool isSync)
        {
            if (handlerType == null) throw new ArgumentNullException(nameof(handlerType));
            if (func == null) throw new ArgumentNullException(nameof(func));
            if (methodInfo == null) throw new ArgumentNullException(nameof(methodInfo));

            var handler = new ExtDirectPollingHandlerRegistryItem
            {
                HandlerType = handlerType,
                Func = func,
                Delegate = methodInfo,
                IsSync = isSync,
                ParameterType = parameterType,
                EventName = eventName
            };

            if (string.IsNullOrWhiteSpace(handler.EventName))
            {
                var attr = handler.Delegate.GetCustomAttribute<ExtDirectEventNameAttribute>();
                if (attr != null) handler.EventName = attr.EventName;
            }
            if (string.IsNullOrWhiteSpace(handler.EventName))
            {
                handler.EventName = ExtDirectConstants.DEFAULT_EVENT_NAME;
            }

            if (!_handlers.Contains(handler))
            {
                _handlers.Add(handler);
            }
        }
    }

    internal sealed class ExtDirectPollingApiOptionsValidator : AbstractValidator<ExtDirectPollingApiOptions>
    {
        public ExtDirectPollingApiOptionsValidator()
        {
#pragma warning disable CA1416 // Validate platform compatibility

            using var provider = CodeDomProvider.CreateProvider("C#");
            RuleFor(row => row.Name)
                .NotNull()
                .NotEmpty()
                .Must(name => provider.IsValidIdentifier(name))
                .WithMessage(opts => string.Format(Properties.Resources.ERR_INVALID_PROVIDER_NAME, opts.Name));

            RuleFor(row => row.Id)
                .NotNull()
                .NotEmpty();

#pragma warning restore CA1416 // Validate platform compatibility
        }
    }
}