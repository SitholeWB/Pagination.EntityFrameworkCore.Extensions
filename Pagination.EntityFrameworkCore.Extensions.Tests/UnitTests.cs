using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pagination.EntityFrameworkCore.Extensions.Tests
{
	public class Tests
	{
		private UsersDbContext _usersDbContext;

		[SetUp]
		public async Task Setup()
		{
			_usersDbContext = GetDatabaseContext();
			await _usersDbContext.Users.AddAsync(new User
			{
				Firstname = "Bob",
				Id = Guid.NewGuid(),
				Lastname = "Smith"
			});
			await _usersDbContext.Users.AddAsync(new User
			{
				Firstname = "Alice",
				Id = Guid.NewGuid(),
				Lastname = "Cool"
			});
			await _usersDbContext.Users.AddAsync(new User
			{
				Firstname = "Joe",
				Id = Guid.NewGuid(),
				Lastname = "Hudla"
			});
			await _usersDbContext.SaveChangesAsync();
		}

		[Test]
		public async Task AsPaginationAsync_Given_ConverUserToUserViewModel_ShouldReturnExpected()
		{
			var people = await _usersDbContext.Users.AsPaginationAsync(1, 2);
			var peopleView = await _usersDbContext.Users.AsPaginationAsync(1, 2, ConverUserToUserViewModel);
			Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
			Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
		}

		private UserViewModel ConverUserToUserViewModel(User user)
		{
			return new UserViewModel
			{
				Firstname = user.Firstname + " ---view",
				Id = user.Id,
				Lastname = user.Lastname
			};
		}

		private UsersDbContext GetDatabaseContext()
		{
			var options = new DbContextOptionsBuilder<UsersDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;
			var databaseContext = new UsersDbContext(options);
			databaseContext.Database.EnsureCreated();
			return databaseContext;
		}

		public class UserViewModel
		{
			public Guid Id { get; set; }
			public string Firstname { get; set; }
			public string Lastname { get; set; }
		}
	}
}