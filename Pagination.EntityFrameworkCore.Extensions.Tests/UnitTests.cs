using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        public void PaginationAsync_Given_ShouldReturnZeroExpected()
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
        public void PaginationAsync_Given10Limit_ShouldReturnTwoResultsAndZeroPages()
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
        public void PaginationAsync_GivenFalse_ShouldReturnTwoResultsAndZeroPages()
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
        public void GetPagination_Given_ConvertUserToUserViewModel_ShouldReturnZeroExpected()
        {
            var paginated = default(Pagination<UserViewModel>);
            Assert.DoesNotThrow(() =>
            {
                paginated = Pagination<UserViewModel>.GetPagination(
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
        public void GetPagination_Given_ConvertUserToUserViewModel_ShouldReturnOneExpected()
        {
            var paginated = default(Pagination<UserViewModel>);
            Assert.DoesNotThrow(() =>
            {
                paginated = Pagination<UserViewModel>.GetPagination(
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
        public void GetPaginationAsync_Given_ConvertUserToUserViewModel_ShouldReturnZeroExpected()
        {
            var paginated = default(Pagination<UserViewModel>);
            Assert.DoesNotThrowAsync(async () =>
            {
                paginated = await Pagination<UserViewModel>.GetPaginationAsync(
                    new User[] {
                        new User {
                            Firstname = "Jobe",
                            Lastname = "Sithole",
                            Id= Guid.NewGuid(),
                        }
                    }, 0, ConverUserToUserViewModelAsync, 1, 0, true);
            });

            Assert.AreEqual(0, paginated.TotalItems);
            Assert.AreEqual(0, paginated.TotalPages);
            Assert.AreEqual(0, paginated.Results.Count());
        }

        [Test]
        public void GetPaginationAsync_Given_ConvertUserToUserViewModel_ShouldReturnOneExpected()
        {
            var paginated = default(Pagination<UserViewModel>);
            Assert.DoesNotThrowAsync(async () =>
            {
                paginated = await Pagination<UserViewModel>.GetPaginationAsync(
                    new User[] {
                        new User {
                            Firstname = "Jobe",
                            Lastname = "Sithole",
                            Id= Guid.NewGuid(),
                        }
                    }, 1, ConverUserToUserViewModelAsync, 1, 10, true);
            });

            Assert.AreEqual(1, paginated.TotalItems);
            Assert.AreEqual(1, paginated.TotalPages);
            Assert.AreEqual(1, paginated.Results.Count());
        }

        [Test]
        public async Task AsPaginationAsync_Given_ConvertUserToUserViewModel_ShouldReturnZeroExpected()
        {
            var people = await _usersDbContext.Users.AsPaginationAsync(1, 0);
            var peopleView = await _usersDbContext.Users.AsPaginationAsync(1, 0, x => ConverUserToUserViewModel(x));
            Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
            Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
        }

        [Test]
        public async Task AsPaginationAsync_Given_ConvertUserToUserViewModel_ShouldReturnExpected()
        {
            var people = await _usersDbContext.Users.AsPaginationAsync(1, 2);
            var peopleView = await _usersDbContext.Users.AsPaginationAsync(1, 2, ConverUserToUserViewModel);
            Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
            Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
        }

        [Test]
        public async Task AsPaginationAsync_Given_ConvertUserToUserViewModelAsync_ShouldReturnExpected()
        {
            var people = await _usersDbContext.Users.AsPaginationAsync(1, 2);
            var peopleView = await _usersDbContext.Users.AsPaginationAsync(1, 2, ConverUserToUserViewModelAsync);
            Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
            Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
        }

        [Test]
        public void AsPagination_Given_ConvertUserToUserViewModel_ShouldReturnExpected()
        {
            var people = _usersDbContext.Users.AsPagination(1, 2);
            var peopleView = _usersDbContext.Users.AsPagination(1, 2, ConverUserToUserViewModel);
            Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
            Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
        }

        [Test]
        public void AsPagination_GivenSearchAndOrder_ConvertUserToUserViewModel_ShouldReturnFilteredAndSorted()
        {
            var people = _usersDbContext.Users.AsQueryable().AsPagination<User>(1, 2, x => x.Firstname.Contains("Joe"));
            var peopleView = _usersDbContext.Users.AsPagination(1, 2, x => x.Firstname.Contains("Joe"), ConverUserToUserViewModel, nameof(User.Firstname), true);
            Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
            Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
            Assert.AreEqual(0, peopleView.Results.Count(x => !x.Firstname.Contains("Joe")));
        }

        [Test]
        public async Task AsPaginationAsync_Given_ConvertUserToUserViewModel_ShouldReturnFilteredAndSorted()
        {
            var people = await _usersDbContext.Users.AsPaginationAsync<User>(1, 2, x => x.Firstname.Contains("Joe"));
            var peopleView = await _usersDbContext.Users.AsPaginationAsync(1, 2, x => x.Firstname.Contains("Joe"), ConverUserToUserViewModel);
            Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
            Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
            Assert.AreEqual(0, peopleView.Results.Count(x => !x.Firstname.Contains("Joe")));
        }

        [Test]
        public async Task AsPaginationAsync_Given_ConvertUserToUserViewModelAsync_ShouldReturnFilteredAndSorted()
        {
            var people = await _usersDbContext.Users.AsPaginationAsync<User>(1, 2, x => x.Firstname.Contains("Joe"));
            var peopleView = await _usersDbContext.Users.AsPaginationAsync(1, 2, x => x.Firstname.Contains("Joe"), ConverUserToUserViewModelAsync);
            Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
            Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
            Assert.AreEqual(0, peopleView.Results.Count(x => !x.Firstname.Contains("Joe")));
        }

        //DbContext
        [Test]
        public async Task AsPaginationAsync_DbContext_Given_ConvertUserToUserViewModel_ShouldReturnExpected()
        {
            var people = await _usersDbContext.AsPaginationAsync<User>(1, 2);
            var peopleView = await _usersDbContext.AsPaginationAsync<User, UserViewModel>(1, 2, ConverUserToUserViewModel);
            Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
            Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
        }

        [Test]
        public async Task AsPaginationAsync_DbContext_Given_ConvertUserToUserViewModelAsync_ShouldReturnExpected()
        {
            var people = await _usersDbContext.AsPaginationAsync<User>(1, 2);
            var peopleView = await _usersDbContext.AsPaginationAsync<User, UserViewModel>(1, 2, ConverUserToUserViewModelAsync);
            Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
            Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
        }

        [Test]
        public async Task AsPaginationAsync_DbContext_Given_ConvertUserToUserViewModelAsync_ShouldReturnFiltered()
        {
            var peopleView = await _usersDbContext.AsPaginationAsync<User, AuthUserViewModel>(1, 2, x => x.Firstname.Contains("Joe"), x => ConverUserToUserViewModelAsync(x, new AuthUser { Firstname = "Joe" }));
            Assert.AreEqual(1, peopleView.TotalItems);
            Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
        }

        [Test]
        public void AsPagination_DbContext_Given_ConvertUserToUserViewModel_ShouldReturnExpected()
        {
            var people = _usersDbContext.AsPagination<User>(1, 2);
            var peopleView = _usersDbContext.AsPagination<User, UserViewModel>(1, 2, ConverUserToUserViewModel);
            Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
            Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
        }

        [Test]
        public void AsPagination_DbContext_GivenSearchAndOrder_ConvertUserToUserViewModel_ShouldReturnFilteredAndSorted()
        {
            var people = _usersDbContext.AsPagination<User>(1, 2, x => x.Firstname.Contains("Joe"));
            var peopleView = _usersDbContext.AsPagination<User, UserViewModel>(1, 2, x => x.Firstname.Contains("Joe"), ConverUserToUserViewModel, nameof(User.Firstname), true);
            Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
            Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
            Assert.AreEqual(0, peopleView.Results.Count(x => !x.Firstname.Contains("Joe")));
        }

        [Test]
        public async Task AsPaginationAsync_DbContext_Given_ConvertUserToUserViewModel_ShouldReturnFilteredAndSorted()
        {
            var people = await _usersDbContext.AsPaginationAsync<User>(1, 2, x => x.Firstname.Contains("Joe"));
            var peopleView = await _usersDbContext.AsPaginationAsync<User, UserViewModel>(1, 2, x => x.Firstname.Contains("Joe"), ConverUserToUserViewModel);
            Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
            Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());
            Assert.AreEqual(0, peopleView.Results.Count(x => !x.Firstname.Contains("Joe")));
        }

        [Test]
        public void AsPagination_Given_ConvertUserToUserViewModel_ShouldConvertToJson()
        {
            var people = _usersDbContext.AsPagination<User>(1, 2);
            var peopleView = _usersDbContext.AsPagination<User, UserViewModel>(1, 2, ConverUserToUserViewModel);
            Assert.AreEqual(people.TotalItems, peopleView.TotalItems);
            Assert.AreEqual(peopleView.Results.Count(x => x.Firstname.Contains("view")), peopleView.Results.Count());

            var jsonPeople = JsonConvert.SerializeObject(people);
            var jsonPeopleView = JsonConvert.SerializeObject(peopleView);

            Assert.IsTrue(jsonPeople.Contains("TotalItems"));
            Assert.IsTrue(jsonPeople.Contains("CurrentPage"));
            Assert.IsTrue(jsonPeople.Contains("NextPage"));
            Assert.IsTrue(jsonPeople.Contains("PreviousPage"));
            Assert.IsTrue(jsonPeople.Contains("TotalPages"));
            Assert.IsTrue(jsonPeople.Contains("Results"));
            Assert.IsTrue(jsonPeople.Contains("Bob"));
            Assert.IsTrue(jsonPeople.Contains("Smith"));
            Assert.IsTrue(jsonPeople.Contains("Alice"));
            Assert.IsTrue(jsonPeople.Contains("Cool"));

            Assert.IsTrue(jsonPeopleView.Contains("TotalItems"));
            Assert.IsTrue(jsonPeopleView.Contains("CurrentPage"));
            Assert.IsTrue(jsonPeopleView.Contains("NextPage"));
            Assert.IsTrue(jsonPeopleView.Contains("PreviousPage"));
            Assert.IsTrue(jsonPeopleView.Contains("TotalPages"));
            Assert.IsTrue(jsonPeopleView.Contains("Results"));
            Assert.IsTrue(jsonPeopleView.Contains("Bob"));
            Assert.IsTrue(jsonPeopleView.Contains("Smith"));
            Assert.IsTrue(jsonPeopleView.Contains("Alice"));
            Assert.IsTrue(jsonPeopleView.Contains("Cool"));
            Assert.IsTrue(jsonPeopleView.Contains("---view"));

            var peopleObj = JsonConvert.DeserializeObject<Pagination<User>>(jsonPeople);
            var peopleViewObj = JsonConvert.DeserializeObject<Pagination<UserViewModel>>(jsonPeopleView);

            Assert.AreEqual(people.TotalItems, peopleObj.TotalItems);
            Assert.AreEqual(people.CurrentPage, peopleObj.CurrentPage);
            Assert.AreEqual(people.NextPage, peopleObj.NextPage);
            Assert.AreEqual(people.PreviousPage, peopleObj.PreviousPage);
            Assert.AreEqual(people.TotalPages, peopleObj.TotalPages);
            Assert.AreEqual(people.Results.Count(), peopleObj.Results.Count());
            Assert.Greater(peopleObj.Results.Count(), 0);
            Assert.IsTrue(peopleObj.Results.Any(x => x.Firstname.Contains("Bob") && x.Lastname.Contains("Smith")));
            Assert.IsTrue(peopleObj.Results.Any(x => x.Firstname.Contains("Alice") && x.Lastname.Contains("Cool")));
            Assert.IsFalse(peopleObj.Results.Any(x => x.Id.Equals(Guid.Empty)));

            Assert.AreEqual(peopleView.TotalItems, peopleViewObj.TotalItems);
            Assert.AreEqual(peopleView.CurrentPage, peopleViewObj.CurrentPage);
            Assert.AreEqual(peopleView.NextPage, peopleViewObj.NextPage);
            Assert.AreEqual(peopleView.PreviousPage, peopleViewObj.PreviousPage);
            Assert.AreEqual(peopleView.TotalPages, peopleViewObj.TotalPages);
            Assert.AreEqual(peopleView.Results.Count(), peopleViewObj.Results.Count());
            Assert.Greater(peopleViewObj.Results.Count(), 0);
            Assert.IsTrue(peopleViewObj.Results.Any(x => x.Firstname.Contains("Bob") && x.Lastname.Contains("Smith")));
            Assert.IsTrue(peopleViewObj.Results.Any(x => x.Firstname.Contains("Alice") && x.Lastname.Contains("Cool")));
            Assert.IsTrue(peopleViewObj.Results.Any(x => x.Firstname.Contains("---view")));
            Assert.IsFalse(peopleViewObj.Results.Any(x => x.Id.Equals(Guid.Empty)));
            Assert.IsFalse(peopleViewObj.Results.Any(x => x.Id.Equals(Guid.Empty)));
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

        private async Task<UserViewModel> ConverUserToUserViewModelAsync(User user)
        {
            var seconds = new Random().Next(2, 7);
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            return new UserViewModel
            {
                Firstname = user.Firstname + " ---view",
                Id = user.Id,
                Lastname = user.Lastname
            };
        }

        private async Task<AuthUserViewModel> ConverUserToUserViewModelAsync(User user, AuthUser authUser)
        {
            var seconds = new Random().Next(2, 7);
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            var personMatched = await _usersDbContext.Users.FirstOrDefaultAsync(x => x.Firstname == authUser.Firstname);
            return new AuthUserViewModel
            {
                Firstname = user.Firstname + " ---view",
                Id = user.Id,
                Lastname = user.Lastname,
                IsAuthUser = user.Id.Equals(personMatched?.Id)
            };
        }

        private UsersDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<UsersDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new UsersDbContext(options);
            return databaseContext;
        }

        public class AuthUser
        {
            public string Firstname { get; set; }
        }

        public class UserViewModel
        {
            public Guid Id { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
        }

        public class AuthUserViewModel : UserViewModel
        {
            public bool IsAuthUser { get; set; }
        }
    }
}