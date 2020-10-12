using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Honamic.Redirector.Sample
{
    public class DbRedirectorStorage : IRedirectorStorage
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public DbRedirectorStorage(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public List<RedirectObject> GetAll()
        {
            return _applicationDbContext.Redirects.Select(r => new RedirectObject
            {
                Id = r.Id,
                Order = r.Order,
                Destination = r.Destination,
                HttpCode = r.HttpCode,
                Path = r.Path,
                Type = r.Type,
            }).ToList();
        }
    }
}
