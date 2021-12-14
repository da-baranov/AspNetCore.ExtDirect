using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
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

            //
            // Registering ExtDirect remoting handlers
            //
            services.AddExtDirectRemotingApi(options =>
            {
                options.AddHandler<RemotingChatService>("Chat");
                options.AddHandler<RemotingTestHandler>("Test");
            });

            services.AddExtDirectRemotingApi(options =>
            {
                options.Name = "CALCULATOR_API_1";
                options.Namespace = "Calculator1";
                options.AddHandler<RemotingCalculatorService>("Calculator");
            });

            services.AddExtDirectRemotingApi(options =>
            {
                options.Name = "CALCULATOR_API_2";
                options.Namespace = "Calculator2";
                options.AddHandler<RemotingCalculatorService>("Calculator");
            });

            //
            // Registering polling handlers
            // 
            services.AddExtDirectPollingApi(options =>
            {
                options.Name = "POLLING_DATA_API";
                options.AddHandler<PollingService>((sender) => sender.GetEvents(), "ondata");
            });
            services.AddExtDirectPollingApi(options =>
            {
                options.Name = "POLLING_CHAT_API";
                options.AddHandler<RemotingChatService>((sender) => sender.GetEvents(), "onmessage");
            });
            services.AddExtDirectPollingApi(options =>
            {
                options.Name = "POLLING_TEST_API";
                options.AddHandler<PollingTestHandler, Person>((sender, person) => sender.GetEvents(person), "ondata");
            });

            //
            // Test DbContext
            //
            services.AddDbContext<DemoDbContext>(options =>
            {
                options.UseSqlite("Data Source=Data/Demo.db");
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