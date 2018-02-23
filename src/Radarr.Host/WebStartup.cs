using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Radarr.Host
{
    public class WebStartup

    {
        public WebStartup(IHostingEnvironment env, ILoggerFactory logger)
        {

        }

        public void ConfigureServices(IServiceCollection services)
        {
        
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory logger, IConfiguration configuration)
        {
            app.Run(context => context.Response.WriteAsync("Hoi"));
        }
    }
}