using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;

namespace Honamic.Redirector.Rules
{
    internal class RedirectToCanonicalUrlRule : IRule
    {
        private int StatusCode { get; set; }
        private bool _forceLowercaseUrls;
        private TrailingSlashAction _trailingSlash;

        public RedirectToCanonicalUrlRule(int statusCode, bool forceLowercaseUrls, TrailingSlashAction trailingSlash)
        {
            StatusCode = statusCode;
            _forceLowercaseUrls = forceLowercaseUrls;
            _trailingSlash = trailingSlash;
        }

        public virtual void ApplyRule(RewriteContext context)
        {
            var request = context.HttpContext.Request;

            if (!request.Method.Equals("GET", StringComparison.InvariantCultureIgnoreCase))
                return;


            var absoluteUrl = HttpUtility.UrlDecode(request.Path.Value.ToString(CultureInfo.InvariantCulture));

            if (IsFileRequest(absoluteUrl))
                return;

            var lowercasePassed = !_forceLowercaseUrls;

            if (_forceLowercaseUrls)
            {
                lowercasePassed = absoluteUrl == absoluteUrl.ToLowerInvariant();
            }

            bool trailingSlashPassed;

            switch (_trailingSlash)
            {
                case TrailingSlashAction.NoAction:
                    trailingSlashPassed = true;
                    break;
                case TrailingSlashAction.ForceToStrip:
                    trailingSlashPassed = !absoluteUrl.EndsWith("/");
                    break;
                case TrailingSlashAction.ForceToAppend:
                    trailingSlashPassed = absoluteUrl.EndsWith("/");
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(_trailingSlash), (int)_trailingSlash, _trailingSlash.GetType());
            }



            if (!trailingSlashPassed || !lowercasePassed)
            {
                string newUrl = GenereteUrl(request);
                var response = context.HttpContext.Response;
                response.StatusCode = StatusCode;
                response.Headers[HeaderNames.Location] = newUrl.ToString();
                context.Result = RuleResult.EndResponse;
                context.Logger.RedirectedToHttps();
            }
        }

        private string GenereteUrl(Microsoft.AspNetCore.Http.HttpRequest request)
        {
            var PathBase = request.PathBase.ToString();
            var Path = request.Path.ToString();

            if (_forceLowercaseUrls)
            {
                PathBase = PathBase.ToLowerInvariant();
                Path = Path.ToLowerInvariant();
            }

            switch (_trailingSlash)
            {
                case TrailingSlashAction.ForceToStrip:
                    Path = Path.TrimEnd('/');
                    break;
                case TrailingSlashAction.ForceToAppend:
                    if (!Path.EndsWith("/"))
                    {
                        Path = Path + "/";
                    }
                    break;
            }

            var newUrl = UriHelper.BuildRelative(PathBase, Path, request.QueryString);

            return newUrl;
        }

        private bool IsFileRequest(string absoluteUrl)
        {
            return !string.IsNullOrEmpty(System.IO.Path.GetExtension(absoluteUrl));
        }
    }
}
