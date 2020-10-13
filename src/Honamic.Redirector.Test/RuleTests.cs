using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Honamic.Redirector.Test
{
    public class RuleTests
    {
        [Theory]
        [InlineData("https://www.honamic.dev/", "https://honamic.dev/")]
        [InlineData("http://www.honamic.dev/pages/contact-us/", "http://honamic.dev/pages/contact-us/")]
        [InlineData("https://www.honamic.dev:8081/posts/?q=1&Param2=Test&param3=هنامیک", "https://honamic.dev:8081/posts/?q=1&Param2=Test&param3=هنامیک")]
        public async Task CheckRedirectToNonWww(string requestUri, string redirectUri)
        {
            var options = new RewriteOptions()
                .AddRedirectToNonWww();

            using var host = new HostBuilder()
                .ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder
                    .UseTestServer()
                    .Configure(app =>
                    {
                        app.UseRewriter(options);
                    });
                }).Build();

            await host.StartAsync();

            var server = host.GetTestServer();

            var response = await server.CreateClient().GetAsync(new Uri(requestUri));

            Assert.Equal(redirectUri, HttpUtility.UrlDecode(response.Headers?.Location?.OriginalString ?? ""));
            Assert.Equal(StatusCodes.Status307TemporaryRedirect, (int)response.StatusCode);
        }



        [Theory]
        [InlineData("http://honamic.dev/Pages/contact-us/", "/pages/contact-us/")]
        [InlineData("https://honamic.dev/تست-حروف-فارسی-AspNet", "/تست-حروف-فارسی-aspnet")]
        [InlineData("http://honamic.dev:8081/Posts/?q=1&Param2=Test", "/posts/?q=1&Param2=Test")]
        public async Task CheckRedirectToLowercase(string requestUri, string redirectUri)
        {
            var options = new RewriteOptions().AddRedirectToLowercase();
            using var host = new HostBuilder()
                .ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder
                    .UseTestServer()
                    .Configure(app =>
                    {
                        app.UseRewriter(options);
                    });
                }).Build();

            await host.StartAsync();

            var server = host.GetTestServer();

            var response = await server.CreateClient().GetAsync(new Uri(requestUri));

            Assert.Equal(redirectUri, HttpUtility.UrlDecode(response.Headers.Location.OriginalString));
            Assert.Equal(StatusCodes.Status307TemporaryRedirect, (int)response.StatusCode);
        }


        [Theory]
        [InlineData("https://honamic.dev/pages/contact-us", "/pages/contact-us/", true, TrailingSlashAction.ForceToAppend)]
        [InlineData("https://honamic.dev/PaGes/contact-us/", "/pages/contact-us", true, TrailingSlashAction.ForceToStrip)]
        [InlineData("https://honamic.dev/تست-حروف-فارسی-ASPnet/", "/تست-حروف-فارسی-aspnet", true, TrailingSlashAction.ForceToStrip)]
        [InlineData("http://honamic.dev:8081/posts/?q=1&Param2=Test", "/posts?q=1&Param2=Test", true, TrailingSlashAction.ForceToStrip)]
        public async Task CheckStripTrailingSlash(string requestUri, string redirectUri, bool forceLowercaseUrls, TrailingSlashAction trailingSlash)
        {
            var options = new RewriteOptions().AddCanonicalUrl(StatusCodes.Status307TemporaryRedirect, forceLowercaseUrls, trailingSlash);
            using var host = new HostBuilder()
                .ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder
                    .UseTestServer()
                    .Configure(app =>
                    {
                        app.UseRewriter(options);
                    });
                }).Build();

            await host.StartAsync();

            var server = host.GetTestServer();

            var response = await server.CreateClient().GetAsync(new Uri(requestUri));

            Assert.Equal(redirectUri, HttpUtility.UrlDecode(response.Headers.Location.OriginalString));
        }
    }
}
