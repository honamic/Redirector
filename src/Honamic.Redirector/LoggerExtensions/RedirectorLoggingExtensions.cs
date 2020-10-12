using System;
using Microsoft.Extensions.Logging;

namespace Honamic.Redirector
{
    internal static class RewriteMiddlewareLoggingExtensions
    {
        private static readonly Action<ILogger, string, string, Exception> _redirected;

        static RewriteMiddlewareLoggingExtensions()
        {
            _redirected = LoggerMessage.Define<string, string>(
                            LogLevel.Debug,
                            new EventId(1, "Redirected"),
                            "Request redirected from {sourceUrl} to {currentUrl}");
        }

        public static void Redirected(this ILogger logger, string sourceUrl, string destinationUrl)
        {
            _redirected(logger, sourceUrl, destinationUrl, null);
        }

    }
}