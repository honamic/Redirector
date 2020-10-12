using System.Collections.Generic;
using System.Threading.Tasks;

namespace Honamic.Redirector.Test
{
    public class SampleRedirectorStorage : IRedirectorStorage
    {
        public Task<IList<RedirectObject>> GetAll()
        {
            IList<RedirectObject> list = new List<RedirectObject>();

            list.Add(new RedirectObject
            {
                Id = "1",
                Type = RedirectType.Path,
                Path = "/posts",
                Destination = "/weblog",
                Order = 1,
            });

            list.Add(new RedirectObject
            {
                Id = "2",
                Type = RedirectType.Regex,
                Path = "/posts/([0-9]*)/(.*)",
                Destination = "/weblog/$1/$2",
                Order = 1,
            });


            return Task.FromResult(list);
        }
    }

}
