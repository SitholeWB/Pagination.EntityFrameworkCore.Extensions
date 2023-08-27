using Microsoft.EntityFrameworkCore;

namespace WebApi.Data
{
    public class WebApiContext : DbContext
    {
        public WebApiContext(DbContextOptions<WebApiContext> options)
            : base(options)
        {
            Database.Migrate();
        }

        public DbSet<WebApi.Entities.Country> Country { get; set; } = default!;
        public DbSet<WebApi.Entities.Person> Person { get; set; } = default!;
    }
}