using FluentValidation;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AspNetCore.ExtDirect
{
    public static class ExtDirectExtensions
    {
        /// <summary>
        /// Adds AspNetCore.ExtDirect services to the specified IServiceCollection.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to</param>
        /// <param name="configure">An action to configure Ext Direct options</param>
        /// <returns>A reference to this instance after the operation has completed</returns>
        public static IServiceCollection AddExtDirect(this IServiceCollection services, Action<ExtDirectOptions> configure = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // Localization
            services.AddLocalization();

            // Configuration
            var options = new ExtDirectOptions();
            configure?.Invoke(options);
            new ExtDirectOptionsValidator().ValidateAndThrow(options);
            services.AddSingleton(options);

            // Repository
            services.AddSingleton(new ExtDirectHandlerRepository(services.BuildServiceProvider()));

            // Batch manager
            services.AddScoped<IExtDirectBatchService, ExtDirectBatchService>(); // Public

            return services;
        }

        /// <summary>
        /// Registers an Ext Direct event (polling) provider 
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to</param>
        /// <param name="configure">An action to configure Ext Direct polling API options</param>
        /// <remarks>There MAY be more than one Event Provider per Server; in that case each Event Provider MUST have a separate Service URI</remarks>
        /// <returns>A reference to this instance after the operation has completed</returns>
        public static IServiceCollection AddExtDirectPollingApi(
            this IServiceCollection services,
            Action<ExtDirectPollingApiOptions> configure)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var options = new ExtDirectPollingApiOptions();
            configure?.Invoke(options);
            new ExtDirectPollingApiOptionsValidator().ValidateAndThrow(options);

            var repository = services.GetRepository();
            repository.RegisterPollingHandler(options);
            return services;
        }

        /// <summary>
        /// Registers an Ext Direct remoting handler
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to</param>
        /// <param name="configure">An action to configure Ext Direct Remoting API options</param>
        /// <returns>A reference to this instance after the operation has completed</returns>
        public static IServiceCollection AddExtDirectRemotingApi(this IServiceCollection services, Action<ExtDirectRemotingApiOptions> configure)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var options = new ExtDirectRemotingApiOptions();
            configure?.Invoke(options);
            new ExtDirectRemotingApiOptionsValidator().ValidateAndThrow(options);

            var repository = services.GetRepository();
            repository.RegisterRemotingHandler(options);
            return services;
        }

        /// <summary>
        /// Adds an Ext Direct middleware to the specified IApplicationBuilder
        /// </summary>
        /// <param name="app">The IApplicationBuilder to add the middleware to</param>
        /// <returns>A reference to this instance after the operation has completed</returns>
        public static IApplicationBuilder UseExtDirect(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            // Apply options
            var options = app.ApplicationServices.GetService<ExtDirectOptions>();
            if (options == null)
            {
                throw new ApplicationException(Properties.Resources.ERR_NOT_CONFIGURED);
            }

            app.UseEndpoints(endpoints =>
            {
                var controllerName = nameof(ExtDirectApiController).Replace("Controller", string.Empty);

                // ExtDirect service descriptor (handles HTTP GET)
                endpoints.MapControllerRoute(
                    name: "ExtDirectApi",
                    pattern: options.ClientApiUrl,
                    defaults: new { controller = controllerName, action = nameof(ExtDirectApiController.Index) });

                // Ext Direct remoting controller (handles HTTP POST)
                endpoints.MapControllerRoute(
                    name: "ExtDirectRemoting",
                    pattern: options.RemotingEndpointUrl + "/{providerId}",
                    defaults: new { controller = controllerName, action = nameof(ExtDirectApiController.OnAction) });

                // Ext Direct polling controller (handles HTTP GET)
                endpoints.MapControllerRoute(
                    name: "ExtDirectPolling",
                    pattern: options.PollingEndpointUrl + "/{providerId}",
                    defaults: new { controller = controllerName, action = nameof(ExtDirectApiController.OnEvents) });
            });

            return app;
        }

        private static ExtDirectHandlerRepository GetRepository(this IServiceCollection services)
        {
            var result = services.FirstOrDefault(row => row.ImplementationInstance is ExtDirectHandlerRepository);
            if (result == null)
            {
                throw new ApplicationException(Properties.Resources.ERR_NOT_CONFIGURED);
            }
            var serviceProvider = services.BuildServiceProvider();
            var rv = serviceProvider.GetService<ExtDirectHandlerRepository>();
            return rv;
        }
    }
}