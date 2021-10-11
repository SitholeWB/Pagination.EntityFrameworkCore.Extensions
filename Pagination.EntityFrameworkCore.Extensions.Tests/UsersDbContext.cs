using Microsoft.EntityFrameworkCore;

namespace Pagination.EntityFrameworkCore.Extensions.Tests
{
	public class UsersDbContext : DbContext
	{
		public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }
	}
}
