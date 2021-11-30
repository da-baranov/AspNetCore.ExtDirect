using AspNetCore.ExtDirect.Meta;
using FluentValidation;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect
{
    public class ExtDirectPollingApiOptions
    {
        private readonly Dictionary<Type, object> _handlerTypes = new();

        /// <summary>
        /// List of registered handlers
        /// </summary>
        public IReadOnlyDictionary<Type, object> HandlerTypes => _handlerTypes;

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
        /// Gets or sets the name of the current API. This name should be unique within a web application.
        /// </summary>
        public string Name { get; set; } = "POLLING_API";

        /// <summary>
        /// How often to poll the server-side in milliseconds. Defaults to every 3 seconds.
        /// </summary>
        public int? Interval { get; set; } = 3000;

        /// <summary>
        /// Registers a polling event sync handler that receives arguments of type T1 from query string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="func"></param>
        public void AddHandler<T, T1>(Func<T, T1, IEnumerable<PollResponse>> func)
            where T : class
            where T1: class
        {
            if (!_handlerTypes.ContainsKey(typeof(T)))
            {
                _handlerTypes.Add(typeof(T), func);
            }
        }

        /// <summary>
        /// Registers a polling event sync handler with no arguments
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        public void AddHandler<T>(Func<T, IEnumerable<PollResponse>> func)
            where T : class
        {
            if (!_handlerTypes.ContainsKey(typeof(T)))
            {
                _handlerTypes.Add(typeof(T), func);
            }
        }


        public void AddHandler<T>(Func<T, Task<IEnumerable<PollResponse>>> func)
        {
            if (!_handlerTypes.ContainsKey(typeof(T)))
            {
                _handlerTypes.Add(typeof(T), func);
            }
        }

        public void AddHandler<T, T1>(Func<T, T1, Task<IEnumerable<PollResponse>>> func)
        {
            if (!_handlerTypes.ContainsKey(typeof(T)))
            {
                _handlerTypes.Add(typeof(T), func);
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
                .Must(id => provider.IsValidIdentifier(id))
                .WithMessage(opts => string.Format(Properties.Resources.ERR_INVALID_PROVIDER_NAME, opts.Name));

            RuleFor(row => row.Id)
                .NotNull()
                .NotEmpty();

#pragma warning restore CA1416 // Validate platform compatibility
        }
    }
}