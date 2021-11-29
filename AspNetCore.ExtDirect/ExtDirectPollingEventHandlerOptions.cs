using AspNetCore.ExtDirect.Meta;
using FluentValidation;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect
{
    public class ExtDirectPollingEventHandlerOptions
    {
        private readonly Dictionary<Type, object> _handlerTypes = new();

        public IReadOnlyDictionary<Type, object> HandlerTypes => _handlerTypes;

        public string Id { get; set; } = Utils.Util.Uuid();

        public string Name { get; set; } = "POLLING_API";

        /// <summary>
        /// Registers a polling event sync handler that receives arguments of type T1 from query string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="func"></param>
        public void AddPollingHandler<T, T1>(Func<T, T1, IEnumerable<PollResponse>> func)
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
        public void AddPollingHandler<T>(Func<T, IEnumerable<PollResponse>> func)
            where T : class
        {
            if (!_handlerTypes.ContainsKey(typeof(T)))
            {
                _handlerTypes.Add(typeof(T), func);
            }
        }


        public void AddPollingHandler<T>(Func<T, Task<IEnumerable<PollResponse>>> func)
        {
            if (!_handlerTypes.ContainsKey(typeof(T)))
            {
                _handlerTypes.Add(typeof(T), func);
            }
        }

        public void AddPollingHandler<T, T1>(Func<T, T1, Task<IEnumerable<PollResponse>>> func)
        {
            if (!_handlerTypes.ContainsKey(typeof(T)))
            {
                _handlerTypes.Add(typeof(T), func);
            }
        }
    }

    internal sealed class ExtDirectPollingEventHandlerOptionsValidator : AbstractValidator<ExtDirectPollingEventHandlerOptions>
    {
        public ExtDirectPollingEventHandlerOptionsValidator()
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