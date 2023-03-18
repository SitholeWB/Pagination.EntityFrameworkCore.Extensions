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
		public async Task PaginationAsync_Given_ShouldReturnZeroExpected()
		{
			var paginated = default(Pagination<string>);
			Assert.DoesNotThrow(() =>
			{
				paginated = new Pagination<string>(new string[] { "one", "two" }, 0, 1, 0, true);
			});

			Assert.AreEqual(0, paginated.TotalItems);
			Assert.AreEqual(0, paginated.TotalPages);
			Assert.AreEqual(0, paginated.Results.Count());
		}

		[Test]
		public async Task PaginationAsync_Given10Limit_ShouldReturnTwoResultsAndZeroPages()
		{
			var paginated = default(Pagination<string>);
			Assert.DoesNotThrow(() =>
			{
				paginated = new Pagination<string>(new string[] { "one", "two" }, 0, 1, 10, true);
			});

			Assert.AreEqual(0, paginated.TotalItems);
			Assert.AreEqual(0, paginated.TotalPages);
			Assert.AreEqual(2, paginated.Results.Count());
		}

		[Test]
		public async Task PaginationAsync_GivenFalse_ShouldReturnTwoResultsAndZeroPages()
		{
			var paginated = default(Pagination<string>);
			Assert.DoesNotThrow(() =>
			{
				paginated = new Pagination<string>(new string[] { "one", "two" }, 0, 1, 10, false);
			});

			Assert.AreEqual(0, paginated.TotalItems);
			Assert.AreEqual(0, paginated.TotalPages);
			Assert.AreEqual(2, paginated.Results.Count());
		}

		[Test]
		public async Task PaginationAsync_Given_ConverUserToUserViewModel_ShouldReturnZeroExpected()
		{
			var paginated = default(PaginationAuto<User, UserViewModel>);
			Assert.DoesNotThrow(() =>
			{
				paginated = new PaginationAuto<User, UserViewModel>(
					new User[] {
						new User {
							Firstname = "Jobe",
							Lastname = "Sithole",
							Id= Guid.NewGuid(),
						}
					}, 0, ConverUserToUserViewModel, 1, 0, true);
			});

			Assert.AreEqual(0, paginated.TotalItems);
			Assert.AreEqual(0, paginated.TotalPages);
			Assert.AreEqual(0, paginated.Results.Count());
		}

		[Test]
		public async Task PaginationAsync_Given_ConverUserToUserViewModel_ShouldReturnOneExpected()
		{
			var paginated = default(PaginationAuto<User, UserViewModel>);
			Assert.DoesNotThrow(() =>
			{
				paginated = new PaginationAuto<User, UserViewModel>(
					new User[] {
						new User {
							Firstname = "Jobe",
							Lastname = "Sithole",
							Id= Guid.NewGuid(),
						}
					}, 1, ConverUserToUserViewModel, 1, 10, true);
			});

			Assert.AreEqual(1, paginated.TotalItems);
			Assert.AreEqual(1, paginated.TotalPages);
			Assert.AreEqual(1, paginated.Results.Count());
		}

		[Test]
		public async Task AsPaginationAsync_Given_ConverUserToUserViewModel_ShouldReturnZeroExpected()
		{
			var people = await _usersDbContext.Users.AsPaginationAsync(1, 0);
			var peopleView = await _usersDbContext.Users.AsPaginationAsync(1, 0, ConverUserToUserViewModel);
			Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
			Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
		}

		[Test]
		public async Task AsPaginationAsync_Given_ConverUserToUserViewModel_ShouldReturnExpected()
		{
			var people = await _usersDbContext.Users.AsPaginationAsync(1, 2);
			var peopleView = await _usersDbContext.Users.AsPaginationAsync(1, 2, ConverUserToUserViewModel);
			Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
			Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
		}

		[Test]
		public void AsPagination_Given_ConverUserToUserViewModel_ShouldReturnExpected()
		{
			var people = _usersDbContext.Users.AsPagination(1, 2);
			var peopleView = _usersDbContext.Users.AsPagination(1, 2, ConverUserToUserViewModel);
			Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
			Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
		}

		[Test]
		public void AsPagination_GivenSearchAndOrder_ConverUserToUserViewModel_ShouldReturnFilteredAndSorted()
		{
			var people = _usersDbContext.Users.AsQueryable().AsPagination<User>(1, 2, x => x.Firstname.Contains("Joe"));
			var peopleView = _usersDbContext.Users.AsPagination(1, 2, x => x.Firstname.Contains("Joe"), ConverUserToUserViewModel, nameof(User.Firstname), true);
			Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
			Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
			Assert.AreEqual(0, peopleView.Results.Count(x => !x.Firstname.Contains("Joe")));
		}

		[Test]
		public async Task AsPaginationAsync_Given_ConverUserToUserViewModel_ShouldReturnFilteredAndSorted()
		{
			var people = await _usersDbContext.Users.AsPaginationAsync<User>(1, 2, x => x.Firstname.Contains("Joe"));
			var peopleView = await _usersDbContext.Users.AsPaginationAsync(1, 2, x => x.Firstname.Contains("Joe"), ConverUserToUserViewModel);
			Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
			Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
			Assert.AreEqual(0, peopleView.Results.Count(x => !x.Firstname.Contains("Joe")));
		}

		//DbContext
		[Test]
		public async Task AsPaginationAsync_DbContext_Given_ConverUserToUserViewModel_ShouldReturnExpected()
		{
			var people = await _usersDbContext.AsPaginationAsync<User>(1, 2);
			var peopleView = await _usersDbContext.AsPaginationAsync<User, UserViewModel>(1, 2, ConverUserToUserViewModel);
			Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
			Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
		}

		[Test]
		public void AsPagination_DbContext_Given_ConverUserToUserViewModel_ShouldReturnExpected()
		{
			var people = _usersDbContext.AsPagination<User>(1, 2);
			var peopleView = _usersDbContext.AsPagination<User, UserViewModel>(1, 2, ConverUserToUserViewModel);
			Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
			Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
		}

		[Test]
		public void AsPagination_DbContext_GivenSearchAndOrder_ConverUserToUserViewModel_ShouldReturnFilteredAndSorted()
		{
			var people = _usersDbContext.AsPagination<User>(1, 2, x => x.Firstname.Contains("Joe"));
			var peopleView = _usersDbContext.AsPagination<User, UserViewModel>(1, 2, x => x.Firstname.Contains("Joe"), ConverUserToUserViewModel, nameof(User.Firstname), true);
			Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
			Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
			Assert.AreEqual(0, peopleView.Results.Count(x => !x.Firstname.Contains("Joe")));
		}

		[Test]
		public async Task AsPaginationAsync_DbContext_Given_ConverUserToUserViewModel_ShouldReturnFilteredAndSorted()
		{
			var people = await _usersDbContext.AsPaginationAsync<User>(1, 2, x => x.Firstname.Contains("Joe"));
			var peopleView = await _usersDbContext.AsPaginationAsync<User, UserViewModel>(1, 2, x => x.Firstname.Contains("Joe"), ConverUserToUserViewModel);
			Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
			Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
			Assert.AreEqual(0, peopleView.Results.Count(x => !x.Firstname.Contains("Joe")));
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

		private Ten ConverUserToUserViewModel2(User user)
		{
			return new Ten
			{
			};
		}

		public struct Ten
		{
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