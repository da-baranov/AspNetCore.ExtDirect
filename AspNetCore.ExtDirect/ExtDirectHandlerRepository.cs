using AspNetCore.ExtDirect.Attributes;
using AspNetCore.ExtDirect.Meta;
using AspNetCore.ExtDirect.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AspNetCore.ExtDirect
{
    /// <summary>
    /// For internal use. This class is used as a singleton.
    /// </summary>
    internal sealed class ExtDirectHandlerRepository
    {
        private readonly IStringLocalizer _localizer;
        private readonly IStringLocalizerFactory _localizerFactory;
        private readonly IServiceProvider _services;

        internal ExtDirectHandlerRepository(IServiceProvider services)
        {
            _services = services
                ?? throw new ArgumentNullException(nameof(services));
            _localizerFactory = _services.GetService<IStringLocalizerFactory>();
            _localizer = _localizerFactory.Create(typeof(Properties.Resources));
        }

        // Provider name + polling handler type
        internal Dictionary<string, PollingApi> PollingApis { get; private set; }
            = new Dictionary<string, PollingApi>(StringComparer.InvariantCultureIgnoreCase);

        // Provider name + list of remoting handler types
        internal Dictionary<string, RemotingApi> RemotingApis { get; private set; }
            = new Dictionary<string, RemotingApi>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Registers and ExtDirect polling handler
        /// </summary>
        /// <param name="options">Handler options</param>
        internal void RegisterPollingHandler(ExtDirectPollingEventHandlerOptions options)
        {
            var pollingApi = new PollingApi(options);

            if (PollingApis.ContainsKey(options.Name))
            {
                throw new Exception(_localizer[nameof(Properties.Resources.ERR_DUPLICATE_POLLING_API_NAME), options.Name]);
            }
            this.PollingApis.Add(pollingApi.Name, pollingApi);
        }

        /// <summary>
        /// Registers an ExtDirect action handler
        /// </summary>
        /// <param name="options">Handler options</param>
        internal void RegisterRemotingHandler(ExtDirectActionHandlerOptions options)
        {
            // Validation
            new ExtDirectActionHandlerOptionsValidator().ValidateAndThrow(options);

            // Saving for API
            var remotingApi = new RemotingApi(options);

            if (RemotingApis.ContainsKey(options.Name))
            {
                throw new Exception(_localizer[nameof(Properties.Resources.ERR_DUPLICATE_REMOTING_API_NAME), options.Name]);
            }
            RemotingApis.Add(options.Name, remotingApi);

            foreach (var handlerType in options.HandlerTypes.Keys)
            {
                var actionName = options.HandlerTypes[handlerType];

                if (remotingApi.Actions.ContainsKey(actionName))
                {
                    throw new Exception(_localizer[nameof(Properties.Resources.ERR_DUPLICATE_ACTION), actionName]);
                }

                var remotingAction = new RemotingAction(handlerType)
                {
                    Name = actionName
                };

                remotingApi.Actions.Add(actionName, remotingAction);
            }
        }

        internal void FindExtDirectActionAndMethod(string providerName, string actionName, string method, out Type type, out MethodInfo methodInfo)
        {
            type = null;
            methodInfo = null;

            if (!RemotingApis.TryGetValue(providerName, out RemotingApi remotingApi))
            {
                throw new Exception($"Cannot find ExtDirect provider API handler by name provided (\"{providerName}\")");
            }

            if (!remotingApi.Actions.TryGetValue(actionName, out RemotingAction remotingAction))
            {
                throw new Exception($"Cannot find action \"{actionName}\" within provider \"{providerName}\"");
            }
            type = remotingAction.ActionType;

            var actionMethod = remotingAction[method];
            if (actionMethod == null)
            {
                throw new InvalidOperationException(_localizer[nameof(Properties.Resources.ERR_NO_SUCH_METHOD), method]);
            }

            methodInfo = actionMethod.MethodInfo;
            if (methodInfo.GetCustomAttribute<ExtDirectIgnoreAttribute>(false) != null)
            {
                throw new InvalidOperationException(_localizer[nameof(Properties.Resources.ERR_NO_SUCH_METHOD), method]);
            }   
        }

        internal IEnumerable<RemotingApi> ToRemotingApi()
        {
            return RemotingApis.Values;
        }

        internal IEnumerable<PollingApi> ToPollingApi()
        {
            return PollingApis.Values;
        }

        internal string ToApi(IUrlHelper urlHelper, ExtDirectOptions options)
        {
            var script = new StringBuilder();

            script.AppendLine($"var Ext = Ext || {{}};");

            // Remoting APIs
            foreach (var remotingApi in ToRemotingApi())
            {
                script.AppendLine($"Ext.{remotingApi.Name} = ");
                remotingApi.Url = urlHelper.Content("~/" + options.RemotingEndpointUrl + "/" + remotingApi.Name);
                script.Append(Util.JsonSerialize(remotingApi));
                script.Append(';');
                script.AppendLine();
                script.AppendLine();
            }

            // Polling APIs
            foreach (var pollingApi in ToPollingApi())
            {
                script.AppendLine($"Ext.{pollingApi.Name} = ");
                pollingApi.Url = urlHelper.Content("~/" + options.PollingEndpointUrl + "/" + pollingApi.Name);
                script.Append(Util.JsonSerialize(pollingApi));
                script.Append(';');
                script.AppendLine();
                script.AppendLine();
            }

            return script.ToString();
        }
    }
}