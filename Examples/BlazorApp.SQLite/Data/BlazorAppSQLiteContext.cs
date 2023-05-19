using Microsoft.EntityFrameworkCore;

namespace BlazorApp.SQLite.Data
{
	public class BlazorAppSQLiteContext : DbContext
	{
		public BlazorAppSQLiteContext(DbContextOptions<BlazorAppSQLiteContext> options)
			: base(options)
		{
			Database.Migrate();
		}

		public DbSet<BlazorApp.SQLite.Entities.Country> Country { get; set; } = default!;
	}
}