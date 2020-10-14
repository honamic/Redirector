using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Honamic.Redirector
{
    public class RedirectResult
    {
        public string Destination { get; set; }

        public int HttpCode { get; set; }
    }

    public class RedirectInput
    {
        public PathString Path { get; set; }

        public string Refferer { get; set; }

    }
}
