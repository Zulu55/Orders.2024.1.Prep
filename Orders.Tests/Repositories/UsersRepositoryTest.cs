using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Orders.Backend.Data;
using Orders.Backend.Repositories.Implementations;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;

namespace Orders.Tests.Repositories
{
    [TestClass]
    public class UsersRepositoryTests
    {
        private DataContext _context = null!;
        private UsersRepository _usersRepository = null!;
        private Mock<UserManager<User>> _mockUserManager = null!;
        private Mock<RoleManager<IdentityRole>> _mockRoleManager = null!;
        private Mock<SignInManager<User>> _mockSignInManager = null!;
        private readonly Guid _guid = Guid.NewGuid();

        [TestInitialize]
        public void SetUp()
        {
            // Initialize the in-memory database
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new DataContext(options);

            // Mock the UserManager, RoleManager, SignInManager
            var userStoreMock = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object, null, null, null, null);
            var optionsAccessorMock = new Mock<IOptions<IdentityOptions>>();
            var loggerMock = new Mock<ILogger<SignInManager<User>>>();
            var authenticationSchemeProviderMock = new Mock<IAuthenticationSchemeProvider>();
            var userConfirmationMock = new Mock<IUserConfirmation<User>>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
            _mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object,
                httpContextAccessorMock.Object,
                claimsFactoryMock.Object,
                optionsAccessorMock.Object,
                loggerMock.Object,
                authenticationSchemeProviderMock.Object,
                userConfirmationMock.Object);

            // Create the repository instance
            _usersRepository = new UsersRepository(_context, _mockUserManager.Object, _mockRoleManager.Object, _mockSignInManager.Object);

            // Seed the database with test data
            var country = new Country
            {
                Name = "Country",
                States = new List<State>
                {
                    new State
                    {
                        Name = "State",
                        Cities = new List<City>
                        {
                            new City { Name = "City" }
                        }
                    }
                }
            };
            _context.Countries.Add(country);
            _context.SaveChanges();

            var user1 = new User { Id = "1", FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Address = "Some", Document = "Any", CityId = 1 };
            var user2 = new User { Id = _guid.ToString(), FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com", Address = "Some", Document = "Any", CityId = 1 };
            _context.Users.AddRange(user1, user2);
            _context.SaveChanges();
        }

        [TestMethod]
        public async Task GetAsync_WithEmail_UserExists_ReturnsUser()
        {
            // Arrange
            var email = "john.doe@example.com";

            // Act
            var user = await _usersRepository.GetUserAsync(email);

            // Assert
            Assert.IsNotNull(user);
            Assert.AreEqual("John", user.FirstName);
        }

        [TestMethod]
        public async Task GetAsync_WithEmail_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var email = "nonexistent@example.com";

            // Act
            var user = await _usersRepository.GetUserAsync(email);

            // Assert
            Assert.IsNull(user);
        }

        [TestMethod]
        public async Task GetAsync_WithUserId_UserExists_ReturnsUser()
        {
            // Act
            var user = await _usersRepository.GetUserAsync(_guid);

            // Assert
            Assert.IsNotNull(user);
            Assert.AreEqual("Jane", user.FirstName);
        }

        [TestMethod]
        public async Task GetAsync_WithUserId_UserDoesNotExist_ReturnsFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var user = await _usersRepository.GetUserAsync(userId);

            // Assert
            Assert.IsNull(user);
        }

        [TestMethod]
        public async Task GetAsync_WithPagination_ReturnsUsers()
        {
            // Arrange
            var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10, Filter = "J" };

            // Act
            var result = await _usersRepository.GetAsync(pagination);

            // Assert
            Assert.IsTrue(result.WasSuccess);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(2, result.Result.Count());
        }

        [TestMethod]
        public async Task GetTotalPagesAsync_WithPagination_ReturnsTotalPages()
        {
            // Arrange
            var pagination = new PaginationDTO { Page = 1, RecordsNumber = 1, Filter = "J" };

            // Act
            var result = await _usersRepository.GetTotalPagesAsync(pagination);

            // Assert
            Assert.IsTrue(result.WasSuccess);
            Assert.AreEqual(2, result.Result);
        }

        [TestMethod]
        public async Task GetTotalPagesAsync_WithFilter_ReturnsFilteredTotalPages()
        {
            // Arrange
            var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10, Filter = "John" };

            // Act
            var result = await _usersRepository.GetTotalPagesAsync(pagination);

            // Assert
            Assert.IsTrue(result.WasSuccess);
            Assert.AreEqual(1, result.Result);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }
    }
}