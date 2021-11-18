using FluentValidation;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace AspNetCore.ExtDirect
{
    public class ExtDirectPollingEventHandlerOptions
    {
        private readonly List<Type> _handlerTypes = new();

        public IReadOnlyList<Type> HandlerTypes => _handlerTypes;

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = "POLLING_API";

        public void AddPollingHandler<T>()
            where T : class, IExtDirectPollingEventSource
        {
            if (!_handlerTypes.Contains(typeof(T)))
            {
                _handlerTypes.Add(typeof(T));
            }
        }
    }

    internal sealed class ExtDirectPollingEventHandlerOptionsValidator : AbstractValidator<ExtDirectPollingEventHandlerOptions>
    {
        public ExtDirectPollingEventHandlerOptionsValidator()
        {
#pragma warning disable CA1416 // Validate platform compatibility

            var provider = CodeDomProvider.CreateProvider("C#");
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