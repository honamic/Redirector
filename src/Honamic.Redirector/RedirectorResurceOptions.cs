using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Honamic.Redirector
{
    public class RedirectorResurceOptions
    {
        public RedirectorResurceOptions()
        {
            StatusCode = StatusCodes.Status307TemporaryRedirect;
        }

        public int? StatusCode { get; set; }

        public List<RedirectObject> Items { get; set; }
    }
}
