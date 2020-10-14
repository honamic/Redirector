using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Honamic.Redirector
{
    public interface IRedirectorManager
    {
        RedirectResult Evaluate(HttpRequest request);

        void Reload();

        void AddOrUpdate(List<RedirectObject> redirects);
        
        void Remove(string[] ids);
    }
}