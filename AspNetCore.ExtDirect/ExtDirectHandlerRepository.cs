using AspNetCore.ExtDirect.Attributes;
using AspNetCore.ExtDirect.Meta;
using AspNetCore.ExtDirect.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
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
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _localizerFactory = _services.GetService<IStringLocalizerFactory>();
            _localizer = _localizerFactory.Create(typeof(Properties.Resources));
        }

        // Provider ID + handler type
        internal Dictionary<string, ApiBase> Apis { get; private set; } = new Dictionary<string, ApiBase>(StringComparer.InvariantCultureIgnoreCase);

        internal Dictionary<string, PollingApi> PollingApis
        {
            get
            {
                var result = new Dictionary<string, PollingApi>();
                foreach (var key in Apis.Keys)
                {
                    if (Apis[key] is PollingApi pollingApi)
                    {
                        result.Add(key, pollingApi);
                    }
                }
                return result;
            }
        }

        internal Dictionary<string, RemotingApi> RemotingApis
        {
            get
            {
                var result = new Dictionary<string, RemotingApi>();
                foreach (var key in Apis.Keys)
                {
                    if (Apis[key] is RemotingApi remotingApi)
                    {
                        result.Add(key, remotingApi);
                    }
                }
                return result;
            }
        }

        internal void FindExtDirectActionAndMethod(string providerId,
                                                   string actionName,
                                                   string method,
                                                   out RemotingAction remotingAction,
                                                   out RemotingMethod remotingMethod,
                                                   out Type type,
                                                   out MethodInfo methodInfo)
        {
            type = null;
            methodInfo = null;

            if (!RemotingApis.TryGetValue(providerId, out RemotingApi remotingApi))
            {
                throw new Exception(_localizer[nameof(Properties.Resources.ERR_CANNOT_FIND_API_BY_ID), providerId]);
            }

            if (!remotingApi.Actions.TryGetValue(actionName, out RemotingAction ra))
            {
                throw new Exception(_localizer[nameof(Properties.Resources.ERR_ACTION_NOT_FOUND), actionName, providerId]);
            }
            remotingAction = ra;
            type = remotingAction.ActionType;

            var actionMethod = remotingAction[method];
            remotingMethod = actionMethod ?? throw new InvalidOperationException(_localizer[nameof(Properties.Resources.ERR_NO_SUCH_METHOD), method]);

            methodInfo = actionMethod.MethodInfo;
            if (methodInfo.GetCustomAttribute<ExtDirectIgnoreAttribute>(false) != null)
            {
                throw new InvalidOperationException(_localizer[nameof(Properties.Resources.ERR_NO_SUCH_METHOD), method]);
            }
        }

        /// <summary>
        /// Registers and ExtDirect polling handler
        /// </summary>
        /// <param name="options">Handler options</param>
        internal void RegisterPollingHandler(ExtDirectPollingApiOptions options)
        {
            new ExtDirectPollingApiOptionsValidator().ValidateAndThrow(options);
            var pollingApi = new PollingApi(options);
            ValidateProvider(pollingApi);
            Apis.Add(pollingApi.Id, pollingApi);
        }

        /// <summary>
        /// Registers an ExtDirect action handler
        /// </summary>
        /// <param name="options">Handler options</param>
        internal void RegisterRemotingHandler(ExtDirectRemotingApiOptions options)
        {
            new ExtDirectRemotingApiOptionsValidator().ValidateAndThrow(options);
            var remotingApi = new RemotingApi(options);
            ValidateProvider(remotingApi);
            Apis.Add(remotingApi.Id, remotingApi);

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

        internal string MakeJavaScriptApiDefinition(IUrlHelper urlHelper, ExtDirectOptions options)
        {
            var script = new StringBuilder();

            script.AppendLine($"var Ext = Ext || {{}};");

            // Remoting APIs
            foreach (var remotingApi in ToRemotingApi())
            {
                script.AppendLine($"Ext.{remotingApi.Name} = ");
                remotingApi.Url = urlHelper.Content("~/" + options.RemotingEndpointUrl + "/" + remotingApi.Id);
                script.Append(Util.JsonSerialize(remotingApi));
                script.Append(';');
                script.AppendLine();
                script.AppendLine();
            }

            // Polling APIs
            foreach (var pollingApi in ToPollingApi())
            {
                script.AppendLine($"Ext.{pollingApi.Name} = ");
                pollingApi.Url = urlHelper.Content("~/" + options.PollingEndpointUrl + "/" + pollingApi.Id);
                script.Append(Util.JsonSerialize(pollingApi));
                script.Append(';');
                script.AppendLine();
                script.AppendLine();
            }

            return script.ToString();
        }

        internal IEnumerable<PollingApi> ToPollingApi()
        {
            return Apis.Values.OfType<PollingApi>();
        }

        internal IEnumerable<RemotingApi> ToRemotingApi()
        {
            return Apis.Values.OfType<RemotingApi>();
        }

        internal void ValidateProvider(ApiBase api)
        {
            if (api == null)
            {
                throw new ArgumentNullException(nameof(api));
            }
            if (string.IsNullOrWhiteSpace(api.Id))
            {
                throw new Exception(_localizer[nameof(Properties.Resources.ERR_EMPTY_PROVIDER_ID)]);
            }
            if (string.IsNullOrWhiteSpace(api.Name))
            {
                throw new Exception(_localizer[nameof(Properties.Resources.ERR_EMPTY_PROVIDER_NAME)]);
            }
            if (Apis.ContainsKey(api.Id))
            {
                throw new Exception(_localizer[nameof(Properties.Resources.ERR_DUPLICATE_API_ID), api.Id]);
            }
            if (Apis.Values.Any(row => string.Equals(row.Name, api.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new Exception(_localizer[nameof(Properties.Resources.ERR_DUPLICATE_API_NAME), api.Name]);
            }
        }
    }
}