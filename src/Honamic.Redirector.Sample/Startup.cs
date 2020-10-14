using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Honamic.Redirector.Sample
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

            services.Configure<RedirectorOptions>(Configuration.GetSection("Redirector"));
            services.Configure<RedirectorResurceOptions>(Configuration.GetSection("RedirectorResurce"));
            // services.AddScoped<IRedirectorStorage, DbRedirectorStorage>();
            services.AddRedirector();
            services.AddScoped<ApplicationDbContext>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseRedirector();

            var options = new RewriteOptions()
                .AddRedirectorRoles(app.ApplicationServices);
            //.AddRedirectorRoles(c => c.ForceHttps = true);

            app.UseRewriter(options);

            app.UseEndpoints(endpoints =>
            {
                var opt = endpoints.ServiceProvider.GetRequiredService<IOptionsMonitor<RedirectorOptions>>();

                endpoints.MapGet("{**url}", async context =>
                {
                    await context.Response.WriteAsync("test your url for redirect");
                });
            });
        }
    }
}
