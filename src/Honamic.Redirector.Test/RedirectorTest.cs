using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Honamic.Redirector.Test
{
    public class RedirectorTest
    {
        private readonly IServiceProvider _serviceProvider;

        public RedirectorTest()
        {
            var services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging(cfg => cfg.AddConsole().AddDebug());

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services.Configure<RedirectorOptions>(configuration.GetSection("Redirector"));
            services.Configure<RedirectorResurceOptions>(configuration.GetSection("RedirectorResurce"));
            services.AddRedirector();

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void OptionsRedirectorStorage()
        {
            using (var scop = _serviceProvider.CreateScope())
            {
                var services = scop.ServiceProvider;

                var redirectorManager = services.GetRequiredService<RedirectorManager>();

                var context = HttpContextHelper.MockHttpContext("/posts/11/asp-net-core");

                var result = redirectorManager.Evaluate(context.Object.Request);

                Assert.Equal("/weblog/11/asp-net-core", result?.Destination);
            }
        }

        [Fact]
        public void MultiThreadTest()
        {
            using (var scop = _serviceProvider.CreateScope())
            {
                var services = scop.ServiceProvider;

                var redirectorManager = services.GetRequiredService<RedirectorManager>();

                var contexts = new List<HttpContext>();

                for (int i = 0; i < 1000; i++)
                {
                    var newContext = HttpContextHelper.MockHttpContext("/posts/11/asp-net-core");

                    contexts.Add(newContext.Object);
                }

                Parallel.ForEach(contexts, c => redirectorManager.Evaluate(c.Request));
            }

            Assert.True(true);
        }
    }
}
