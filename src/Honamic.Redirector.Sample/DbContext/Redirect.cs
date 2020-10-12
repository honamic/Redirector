using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Honamic.Redirector.Sample
{
    public class Redirect
    {
        public string Id { get; set; }

        public RedirectType Type { get; set; }

        public string Path { get; set; }

        public string Destination { get; set; }

        public decimal Order { get; set; }

        public int? HttpCode { get; set; }
    }
}
