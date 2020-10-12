using System.Collections.Generic;

namespace Honamic.Redirector
{
    public interface IRedirectorStorage
    {
        List<RedirectObject> GetAll();
    }

}
