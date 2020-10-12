using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Honamic.Redirector.Test
{
    public class UnitTest1
    {
        [Fact]
        public void SampleRedirectorStorage()
        {
            var loggerFactory = new LoggerFactory();

            IRedirectorStorage redirectorStorage = new SampleRedirectorStorage();

            RedirectorManager redirectorManger = new RedirectorManager(redirectorStorage, loggerFactory.CreateLogger<RedirectorManager>());

            var context = mockHttpContext("/posts/11/asp-net-core");

            var result = redirectorManger.Evaluate(context.Object.Request);

            Assert.Equal("/weblog/11/asp-net-core", result?.Destination);
        }

        [Fact]
        public void OptionsRedirectorStorage()
        {
            var loggerFactory = new LoggerFactory();

            var options = new RedirectorOptions();

            options.Items.Add(new RedirectObject
            {
                Id = "1",
                Type = RedirectType.Path,
                Path = "/posts",
                Destination = "/weblog",
                Order = 1,
                HttpCode=302
            });

            options.Items.Add(new RedirectObject
            {
                Id = "2",
                Type = RedirectType.Regex,
                Path = "/posts/([0-9]*)/(.*)",
                Destination = "/weblog/$1/$2",
                Order = 1,
            });

            var monitor = Mock.Of<IOptionsMonitor<RedirectorOptions>>(_ => _.CurrentValue == options);

            IRedirectorStorage redirectorStorage = new OptionsRedirectorStorage(monitor, loggerFactory.CreateLogger<OptionsRedirectorStorage>());

            RedirectorManager redirectorManger = new RedirectorManager(redirectorStorage, loggerFactory.CreateLogger<RedirectorManager>());

            var context = mockHttpContext("/posts/11/asp-net-core");

            var result = redirectorManger.Evaluate(context.Object.Request);

            Assert.Equal("/weblog/11/asp-net-core", result?.Destination);
        }

        [Fact]
        public void MultiThreadTest()
        {
            var loggerFactory = new LoggerFactory();

            IRedirectorStorage redirectorStorage = new SampleRedirectorStorage();

            RedirectorManager redirectorManger = new RedirectorManager(redirectorStorage, loggerFactory.CreateLogger<RedirectorManager>());

            var contexts = new List<HttpContext>();

            for (int i = 0; i < 1000; i++)
            {
                var newContext = mockHttpContext("/posts/11/asp-net-core");

                contexts.Add(newContext.Object);
            }

            Parallel.ForEach(contexts, c => redirectorManger.Evaluate(c.Request));

            Assert.True(true);
        }

        public static Mock<HttpContext> mockHttpContext(string path)
        {
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(request => request.Protocol).Returns("GET");
            mockRequest.Setup(request => request.Method).Returns("1.1");
            mockRequest.Setup(request => request.Scheme).Returns("http");
            mockRequest.Setup(request => request.Host).Returns(new HostString(""));
            mockRequest.Setup(request => request.PathBase).Returns(new PathString("/"));
            mockRequest.Setup(request => request.Path).Returns(new PathString(path));
            mockRequest.Setup(request => request.QueryString).Returns(new QueryString("?query"));
            mockRequest.Setup(request => request.ContentType).Returns("test");
            mockRequest.Setup(request => request.ContentLength).Returns(0);

            var mockContext = new Mock<HttpContext>();
            mockContext.Setup(context => context.Request).Returns(mockRequest.Object);

            return mockContext;
        }

    }
}
