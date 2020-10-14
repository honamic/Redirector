using System;
using System.Text.RegularExpressions;

namespace Honamic.Redirector.Managers
{
    public class CachedRedirectObject : RedirectObject
    {
        public string NormalizedPath { get; private set; }

        public Regex Regex { get; set; }

        public CachedRedirectObject(RedirectObject redirectObject,string normalizedPath)
        {
            Id = redirectObject.Id;
            Type = redirectObject.Type;
            Path = redirectObject.Path;
            Destination = redirectObject.Destination;
            Order = redirectObject.Order;
            Order = redirectObject.Order;
            HttpCode = redirectObject.HttpCode;
            NormalizedPath = normalizedPath;
            Regex = new Regex(redirectObject.Path, RegexOptions.Compiled, TimeSpan.FromSeconds(1));
        }

     
    }
}
