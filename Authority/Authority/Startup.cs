using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleIAM.OpenIdAuthority.Configuration;

namespace Authority
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHsts(options =>
            {
                options.Preload = false;
                options.IncludeSubDomains = false;
                options.MaxAge = TimeSpan.FromMinutes(10); // configure as desired for production
            });
            
            // register custom services here

            services.AddOpenIdAuthority(_configuration, _env);

            services.AddDataProtection()
                .SetApplicationName("openidauthority")
                .PersistKeysToFileSystem(new System.IO.DirectoryInfo(@"/var/dpkeys/"));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, HostingConfig hostingConfig)
        {
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseOpenIdAuthority(env, hostingConfig);
        }
    }
}
