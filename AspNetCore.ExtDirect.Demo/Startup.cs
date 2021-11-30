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

            // Registering ExtDirect remoting handlers. Navigate /ExtDirect.js to 
            services.AddExtDirectRemotingApi(options =>
            {
                options.AddHandler<DemoActionHandler>("Demo");
                options.AddHandler<DemoChatHandler>("Chat");
                options.AddHandler<TestHandler>("Test");
            });

            services.AddExtDirectRemotingApi(options =>
            {
                options.Name = options.Id = "CALCULATOR_API_1";
                options.Namespace = "Calculator1";
                options.AddHandler<CalculatorService>("Calculator");
            });

            services.AddExtDirectRemotingApi(options =>
            {
                options.Name = options.Id = "CALCULATOR_API_2";
                options.Namespace = "Calculator2";
                options.AddHandler<CalculatorService>("Calculator");
            });

            services.AddExtDirectPollingApi(options =>
            {
                options.Name = options.Id = "POLLING_DATA_API";
                options.AddHandler<DemoPollingHandler>((sender) => sender.GetEvents());
            });
            services.AddExtDirectPollingApi(options =>
            {
                options.Name = options.Id = "POLLING_CHAT_API";
                options.AddHandler<DemoChatHandler>((sender) => sender.GetEvents());
            });
            services.AddExtDirectPollingApi(options =>
            {
                options.Name = options.Id = "POLLING_TEST_API";
                options.AddHandler<TestPollingHandler, Person>((sender, person) => sender.GetEvents(person));
            });


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