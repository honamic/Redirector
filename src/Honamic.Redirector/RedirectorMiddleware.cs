using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Honamic.Redirector
{
    public class RedirectorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RedirectorManager redirectorManger;
        private RedirectorOptions _options;
        private readonly ILogger _logger;

        public RedirectorMiddleware(RequestDelegate next,
            RedirectorManager redirectorManger,
            ILoggerFactory loggerFactory,
            IOptionsMonitor<RedirectorOptions> options)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }

            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            _next = next;
            this.redirectorManger = redirectorManger;
            _options = options.CurrentValue;

            _logger = loggerFactory.CreateLogger<RedirectorMiddleware>();
        }


        public Task InvokeAsync(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var result = redirectorManger.Evaluate(context.Request);

            if (result != null)
            {
                context.Response.StatusCode = result.HttpCode;

                context.Response.Headers[HeaderNames.Location] = result.Destination;

                _logger.Redirected(context.Request.Path, result.Destination);

                return Task.CompletedTask;
            }

            return _next(context);
        }

    }
}