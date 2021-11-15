using Microsoft.AspNetCore.Builder;
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
            if (configure != null) configure(options);
            services.AddSingleton(options);

            // Collect ExtDirect handlers
            var repository = new ExtDirectHandlerRepository(services.BuildServiceProvider());
            services.AddSingleton(repository);

            // Transaction manager
            services.AddScoped<IExtDirectTransactionService, ExtDirectTransactionService>(); // Public
            services.AddScoped<ExtDirectTransactionService>(); // Internal

            return services;
        }

        public static IServiceCollection AddExtDirectRemotingHandler<T>(this IServiceCollection services, string actionName = null)
            where T : class
        {
            var repository = GetRepository(services);
            repository.RegisterRemotingHandler(typeof(T), actionName);
            return services;
        }

        public static IServiceCollection AddExtDirectPollingHandler<T>(this IServiceCollection services)
            where T : class, IExtDirectPollingEventSource
        {
            var repository = GetRepository(services);
            repository.RegisterPollingHandler<T>();
            return services;
        }

        public static IApplicationBuilder UseExtDirect(this IApplicationBuilder app)
        {
            // Apply options
            var options = app.ApplicationServices.GetRequiredService<ExtDirectOptions>();
            if (options != null)
            {
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
                        pattern: options.RemotingRouteUrl,
                        defaults: new { controller = controllerName, action = nameof(ExtDirectApiController.Index) });

                    // Ext Direct polling controller (handles HTTP GET)
                    endpoints.MapControllerRoute(
                        name: "ExtDirectPolling",
                        pattern: options.PollingRouteUrl,
                        defaults: new { controller = controllerName, action = nameof(ExtDirectApiController.Events) });
                });
            }

            // Collect handlers
            var repository = app.ApplicationServices.GetService<ExtDirectHandlerRepository>();
            // repository.ScanAssemblies(); - draft

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