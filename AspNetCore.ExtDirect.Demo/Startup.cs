using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.ExtDirect.Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            services.AddExtDirect(options =>
            {
                // TODO: modify options here
            });

            // Registering ExtDirect remoting handlers. Navigate /ExtDirect.js to 
            services.AddExtDirectRemotingApi(options =>
            {
                options.AddActionHandler<DemoActionHandler>("Demo");
                options.AddActionHandler<DemoChatHandler>("Chat");
                options.AddActionHandler<TestHandler>("Test");
            });

            services.AddExtDirectRemotingApi(options =>
            {
                options.Name = "CALCULATOR_API_1";
                options.Namespace = "Calculator1";
                options.AddActionHandler<CalculatorService>("Calculator");
            });

            services.AddExtDirectRemotingApi(options =>
            {
                options.Name = "CALCULATOR_API_2";
                options.Namespace = "Calculator2";
                options.AddActionHandler<CalculatorService>("Calculator");
            });

            services.AddExtDirectPollingApi(options =>
            {
                options.Name = "POLLING_DATA_API";
                options.AddPollingHandler<DemoPollingHandler>((sender) => sender.GetEvents());
            });
            services.AddExtDirectPollingApi(options =>
            {
                options.Name = "POLLING_CHAT_API";
                options.AddPollingHandler<DemoChatHandler>((sender) => sender.GetEvents());
            });
            services.AddExtDirectPollingApi(options =>
            {
                options.Name = "POLLING_TEST_API";
                options.AddPollingHandler<TestPollingHandler, Person>((sender, person) => sender.GetEvents(person));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            app.UseExtDirect();
        }
    }
}