using AspNetCore.ExtDirect.Demo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.ExtDirect.Test
{
    public class TestApplicationFactory : WebApplicationFactory<Dummy>
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            var hostBuilder = Host.CreateDefaultBuilder();
            hostBuilder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseUrls("http://*:10000");

                webBuilder.ConfigureServices(services =>
                {
                    services.AddDistributedMemoryCache();
                    services.AddSession();

                    var mvc = services.AddMvc();
                    mvc.AddNewtonsoftJson();

                    services.AddRazorPages()
                        .AddMvcOptions(options => { })
                        .AddNewtonsoftJson(options =>
                        {
                            options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                            options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                            options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local;
                        });

                    services
                        .AddControllers()
                        .AddNewtonsoftJson(options =>
                        {
                            options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                            options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                            options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local;
                        });

                    services
                        .AddControllersWithViews()
                        .AddNewtonsoftJson(options =>
                        {
                            options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                            options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                            options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local;
                        });

                    services.AddExtDirect();
                    services.AddExtDirectRemotingApi(options =>
                    {
                        options.AddActionHandler<DemoActionHandler>("Demo");
                        options.AddActionHandler<TestHandler>("Test");
                    });
                    services.AddExtDirectPollingApi(options =>
                    {
                        options.AddPollingHandler<TestPollingHandler, TestPerson>(
                            (sender, args) => sender.GetEvents(args)
                        );
                    });
                });

                webBuilder.Configure(app =>
                {
                    app.UseRouting();
                    app.UseRequestLocalization();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapRazorPages();
                        endpoints.MapControllers();
                        // endpoints.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapDefaultControllerRoute();
                    });
                    app.UseExtDirect();
                });
            });

            return hostBuilder;
        }
    }
}