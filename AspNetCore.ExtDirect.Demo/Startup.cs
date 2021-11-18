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
            services.AddExtDirectRemotingApi(options =>
            {
                options.AddActionHandler<DemoActionHandler>("Demo");
                options.AddActionHandler<ChatHandler>("Chat");
            });
            services.AddExtDirectPollingApi(options =>
            {
                options.Name = "POLLING_DATA_API";
                options.AddPollingHandler<DemoPollingHandler>();
            });
            services.AddExtDirectPollingApi(options =>
            {
                options.Name = "POLLING_CHAT_API";
                options.AddPollingHandler<ChatHandler>();
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