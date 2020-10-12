using System;
using System.Globalization;
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;

namespace Honamic.Redirector.Rules
{
    internal class RedirectToLowercaseRule : IRule
    {
        private int StatusCode { get; set; }

        public RedirectToLowercaseRule(int statusCode)
        {
            StatusCode = statusCode;
        }

        public virtual void ApplyRule(RewriteContext context)
        {
            var request = context.HttpContext.Request;

            if (!request.Method.Equals("GET", StringComparison.InvariantCultureIgnoreCase))
                return;

            var absoluteUrl = HttpUtility.UrlDecode(request.Path.Value.ToString(CultureInfo.InvariantCulture));

            var isNotlowercase = absoluteUrl != absoluteUrl.ToLowerInvariant();

            if (isNotlowercase)
            {
                var newUrl = UriHelper.BuildRelative(
                    request.PathBase.ToString().ToLowerInvariant(),
                    request.Path.ToString().ToLowerInvariant(),
                    request.QueryString);

                var response = context.HttpContext.Response;
                response.StatusCode = StatusCode;
                response.Headers[HeaderNames.Location] = newUrl.ToString();
                context.Result = RuleResult.EndResponse;
                context.Logger.RedirectedToHttps();
            }
        }
    }
}
