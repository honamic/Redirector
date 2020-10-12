using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Honamic.Redirector.Sample
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Redirect> Redirects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("RedirectSampleDb");

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Redirect>().HasData(RedirectSamples());

            base.OnModelCreating(modelBuilder);
        }

        public List<Redirect> RedirectSamples()
        {
            var list = new List<Redirect>();

            list.Add(new Redirect
            {
                Id = "1",
                Type = RedirectType.Path,
                Path = "/posts",
                Destination = "/dbblog",
                Order = 1,
            });

            list.Add(new Redirect
            {
                Id = "2",
                Type = RedirectType.Regex,
                Path = "/posts/([0-9]*)/(.*)",
                Destination = "/dbblog/$1/$2",
                Order = 1,
            });

            return list;
        }
    }
}
