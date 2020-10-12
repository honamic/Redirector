using Honamic.Redirector;
using System;


namespace Microsoft.AspNetCore.Builder
{
    public static class RedirectorBuilderExtensions
    {
        /// <summary>
        /// Adds middleware for redirect urls.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> instance this method extends.</param>
        public static IApplicationBuilder UseRedirector(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
             

            return builder.UseMiddleware<RedirectorMiddleware>();

        }
    }
}
