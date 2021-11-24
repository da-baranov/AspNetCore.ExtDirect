using FluentValidation;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;

namespace AspNetCore.ExtDirect
{
    /// <summary>
    /// Options to configure an instance of Ext Direct client RemotingProvider 
    /// </summary>
    /// <see href="https://docs.sencha.com/extjs/7.0.0/guides/backend_connectors/direct/specification.html"/>
    public sealed class ExtDirectActionHandlerOptions
    {
        private readonly Dictionary<Type, string> _handlerTypes = new();

        /// <summary>
        /// A set of Action handlers registered within the Remoting API
        /// </summary>
        public IReadOnlyDictionary<Type, string> HandlerTypes => _handlerTypes;

        /// <summary>
        /// The unique id of the provider. Default value is random UUID string.
        /// </summary>
        public string Id { get; set; } = Utils.Util.Uuid();

        /// <summary>
        /// The unique name of the provider. Default value is "REMOTING_API"
        /// </summary>
        public string Name { get; set; } = "REMOTING_API";

        /// <summary>
        /// Namespace for the Ext Direct RemotingProvider
        /// </summary>
        /// <see href="https://docs.sencha.com/extjs/7.0.0/classic/Ext.direct.RemotingProvider.html#cfg-namespace"/>
        public string Namespace { get; set; }

        /// <summary>
        /// The timeout (in milliseconds) to use for each request
        /// </summary>
        /// <see href="https://docs.sencha.com/extjs/7.0.0/classic/Ext.direct.RemotingProvider.html#cfg-timeout"/>
        public int? Timeout { get; set; }

        /// <summary>
        /// Registers a custom class as an Ext Direct RPC handler
        /// </summary>
        /// <typeparam name="T">Custom action handler type. This class can expose a default public constructor, or a constructor that accepts an instance of IServiceProvider and other ASP.NET web application services</typeparam>
        /// <param name="actionName">By default the library uses custom type name as an Action name. You override this and assign some custom name.</param>
        public void AddActionHandler<T>(string actionName = null)
            where T : class
        {
            var type = typeof(T);

            var internalActionName = type.Name;

            var extDirectActionAttribute = type.GetCustomAttribute<ExtDirectActionAttribute>(false);
            if (extDirectActionAttribute != null && !string.IsNullOrWhiteSpace(extDirectActionAttribute.Name))
            {
                internalActionName = extDirectActionAttribute.Name;
            }

            if (!string.IsNullOrWhiteSpace(actionName))
            {
                internalActionName = actionName;
            }

            if (!_handlerTypes.ContainsKey(type))
            {
                _handlerTypes.Add(type, internalActionName);
            }
            else
            {
                _handlerTypes[type] = internalActionName;
            }
        }
    }

    internal sealed class ExtDirectActionHandlerOptionsValidator : AbstractValidator<ExtDirectActionHandlerOptions>
    {
        public ExtDirectActionHandlerOptionsValidator()
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