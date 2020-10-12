using Honamic.Redirector;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel;

namespace Microsoft.AspNetCore.Rewrite
{
    public static class RewriteOptionsExtensions
    {
        public static RewriteOptions AddRedirectorRoles(this RewriteOptions RewriteOptions, IServiceProvider serviceProvider)
        {
            var optins = serviceProvider.GetRequiredService<IOptions<RedirectorOptions>>();

            return RewriteOptions.AddRedirectorRoles(optins.Value);
        }

        public static RewriteOptions AddRedirectorRoles(this RewriteOptions RewriteOptions, Action<RedirectorOptions> configureOptions)
        {
            var options = new RedirectorOptions();

            configureOptions(options);

            return RewriteOptions.AddRedirectorRoles(options);

        }

        private static RewriteOptions AddRedirectorRoles(this RewriteOptions RewriteOptions, RedirectorOptions options)
        {
            var statusCode = options.RedirectStatusCode;

            if (options.ForceHttps)
            {
                RewriteOptions.AddRedirectToHttps();
            }

            if (options.ForceLowercaseUrls || options.TrailingSlash != TrailingSlashAction.NoAction)
            {
                RewriteOptions.AddCanonicalUrl(statusCode, options.ForceLowercaseUrls, options.TrailingSlash);
            }

            switch (options.WwwMode)
            {
                case WwwModeAction.NoAction:
                    break;
                case WwwModeAction.ForceToWww:
                    RewriteOptions.AddRedirectToWww(statusCode);
                    break;
                case WwwModeAction.ForceToNonWww:
                    RewriteOptions.AddRedirectToNonWww(statusCode);
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(options.WwwMode), (int)options.WwwMode, options.WwwMode.GetType());
            }

            return RewriteOptions;
        }
    }
}