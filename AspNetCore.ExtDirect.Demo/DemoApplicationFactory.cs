using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.ExtDirect.Demo
{
    /// <summary>
    /// An application factory for integration testing 
    /// </summary>
    public sealed class DemoApplicationFactory: WebApplicationFactory<Startup>
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            var result = base.CreateHostBuilder();
            result.ConfigureWebHost(configure =>
            {
                configure.UseUrls("https://localhost:10001;http://localhost:10000");
                configure.UseSetting("https_port", "10001");
            });
            return result;
        }
    }
}