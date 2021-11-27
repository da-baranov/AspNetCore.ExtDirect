using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace AspNetCore.ExtDirect
{
    public static class ExtDirectExtensions
    {
        public static IServiceCollection AddExtDirect(this IServiceCollection services, Action<ExtDirectOptions> configure = null)
        {
            var options = new ExtDirectOptions();

            // Localization
            services.AddLocalization();

            // Configuration
            configure?.Invoke(options);
            new ExtDirectOptionsValidator().ValidateAndThrow(options);
            services.AddSingleton(options);

            // Action context accessor
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            // Collect ExtDirect handlers
            var repository = new ExtDirectHandlerRepository(services.BuildServiceProvider());
            services.AddSingleton(repository);

            // Transaction manager
            services.AddScoped<IExtDirectTransactionService, ExtDirectTransactionService>(); // Public
            services.AddScoped<ExtDirectTransactionService>(); // Internal

            return services;
        }

        public static IServiceCollection AddExtDirectRemotingApi(this IServiceCollection services, Action<ExtDirectActionHandlerOptions> configure)
        {
            var options = new ExtDirectActionHandlerOptions();
            configure?.Invoke(options);
            new ExtDirectActionHandlerOptionsValidator().ValidateAndThrow(options);

            var repository = GetRepository(services);
            repository.RegisterRemotingHandler(options);
            return services;
        }

        public static IServiceCollection AddExtDirectPollingApi(
            this IServiceCollection services, 
            Action<ExtDirectPollingEventHandlerOptions> configure)
        {
            var options = new ExtDirectPollingEventHandlerOptions();
            configure?.Invoke(options);
            new ExtDirectPollingEventHandlerOptionsValidator().ValidateAndThrow(options);

            var repository = GetRepository(services);
            repository.RegisterPollingHandler(options);
            return services;
        }

        public static IApplicationBuilder UseExtDirect(this IApplicationBuilder app)
        {
            // Apply options
            var options = app.ApplicationServices.GetService<ExtDirectOptions>();

            if (options == null)
            {
                throw new ApplicationException(Properties.Resources.ERR_NOT_CONFIGURED);
            }

            app.UseEndpoints(endpoints =>
            {
                var controllerName = nameof(ExtDirectApiController).Replace("Controller", "");

                // ExtDirect service descriptor (handles HTTP GET)
                endpoints.MapControllerRoute(
                    name: "ExtDirectApi",
                    pattern: options.RemotingRouteUrl + ".js",
                    defaults: new { controller = controllerName, action = nameof(ExtDirectApiController.Index) });

                // Ext Direct remoting controller (handles HTTP POST)
                endpoints.MapControllerRoute(
                    name: "ExtDirectRemoting",
                    pattern: options.RemotingRouteUrl + "/{providerName}",
                    defaults: new { controller = controllerName, action = nameof(ExtDirectApiController.OnAction) });

                // Ext Direct polling controller (handles HTTP GET)
                endpoints.MapControllerRoute(
                    name: "ExtDirectPolling",
                    pattern: options.PollingRouteUrl + "/{providerName}",
                    defaults: new { controller = controllerName, action = nameof(ExtDirectApiController.OnEvents) });
            });

            var repository = app.ApplicationServices.GetService<ExtDirectHandlerRepository>();
            return app;
        }

        private static ExtDirectHandlerRepository GetRepository(this IServiceCollection services)
        {
            ExtDirectHandlerRepository repository;

            var tmp = services.FirstOrDefault(row => row.ServiceType == typeof(ExtDirectHandlerRepository));
            if (tmp == null)
            {
                repository = new ExtDirectHandlerRepository(services.BuildServiceProvider());
                services.AddSingleton(repository);
            }
            else
            {
                repository = tmp.ImplementationInstance as ExtDirectHandlerRepository;
            }

            return repository;
        }
    }
}