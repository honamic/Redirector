using Honamic.Redirector;
using Honamic.Redirector.Rules;
using System;
using System.ComponentModel;

namespace Microsoft.AspNetCore.Rewrite
{
    public static class RedirectorRewriteOptionsExtensions
    {
        public static RewriteOptions AddRedirectToNonWww(this RewriteOptions options)
        {
            options.AddRedirectToNonWww(307);

            return options;
        }

        public static RewriteOptions AddRedirectToNonWwwPermanent(this RewriteOptions options)
        {
            options.AddRedirectToNonWww(308);

            return options;
        }

        public static RewriteOptions AddRedirectToNonWww(this RewriteOptions options, int statuscode)
        {
            options.Add(new RedirectToNonWwwRule(statuscode));

            return options;
        }

        public static RewriteOptions AddRedirectToLowercase(this RewriteOptions options)
        {
            options.AddRedirectToLowercase(307);

            return options;
        }

        public static RewriteOptions AddRedirectToLowercasePermanent(this RewriteOptions options)
        {
            options.AddRedirectToLowercase(308);

            return options;
        }

        public static RewriteOptions AddRedirectToLowercase(this RewriteOptions options, int statuscode)
        {
            options.Add(new RedirectToLowercaseRule(statuscode));

            return options;
        }

        public static RewriteOptions AddCanonicalUrl(this RewriteOptions options, int statuscode, bool forceLowercaseUrls, TrailingSlashAction trailingSlash)
        {
            options.Add(new RedirectToCanonicalUrlRule(statuscode, forceLowercaseUrls, trailingSlash));

            return options;
        }

        [Obsolete("use AddCanonicalUrl", true)]
        public static RewriteOptions ForceToStripTrailingSlash(this RewriteOptions options)
        {
            options.ForceToStripTrailingSlash(307);

            return options;
        }

        [Obsolete("use AddCanonicalUrl", true)]
        public static RewriteOptions ForceToStripTrailingSlashPermanent(this RewriteOptions options)
        {
            options.ForceToStripTrailingSlash(308);

            return options;
        }

        [Obsolete("use AddCanonicalUrl", true)]
        public static RewriteOptions ForceToStripTrailingSlash(this RewriteOptions options, int statusCode)
        {
            options.AddRedirect("(.*)/$", "$1", statusCode);

            return options;
        }

        [Obsolete("use AddCanonicalUrl", true)]
        public static RewriteOptions ForceToAppendTrailingSlash(this RewriteOptions options)
        {
            options.ForceToAppendTrailingSlash(307);

            return options;
        }

        [Obsolete("use AddCanonicalUrl", true)]
        public static RewriteOptions ForceToAppendTrailingSlashPermanent(this RewriteOptions options)
        {
            options.ForceToAppendTrailingSlash(308);

            return options;
        }

        [Obsolete("use AddCanonicalUrl", true)]
        public static RewriteOptions ForceToAppendTrailingSlash(this RewriteOptions options, int statusCode)
        {
            options.AddRedirect("(.*[^/])$", "$1/", statusCode);

            return options;
        }

    }
}