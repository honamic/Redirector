using Microsoft.AspNetCore.Http;
using Moq;

namespace Honamic.Redirector.Test
{
    public static class HttpContextHelper
    {
        public static Mock<HttpContext> MockHttpContext(string path)
        {
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(request => request.Protocol).Returns("GET");
            mockRequest.Setup(request => request.Scheme).Returns("https");
            mockRequest.Setup(request => request.Host).Returns(new HostString(""));
            mockRequest.Setup(request => request.PathBase).Returns(new PathString("/"));
            mockRequest.Setup(request => request.Path).Returns(new PathString(path));

            var mockContext = new Mock<HttpContext>();
            mockContext.Setup(context => context.Request).Returns(mockRequest.Object);

            return mockContext;
        }
    }
}
