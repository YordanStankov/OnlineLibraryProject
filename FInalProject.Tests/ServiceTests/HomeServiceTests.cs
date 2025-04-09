using System.Security.Claims;
using System.Threading.Tasks;
using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.Services;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace FinalProject.Tests.ServiceTests
{
    [TestFixture]
    public class HomeServiceTests
    {
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private Mock<ILogger<HomeService>> _loggerMock;
        private Mock<ApplicationDbContext> _contextMock;
        private HomeService _homeService;

        [SetUp]
        public void SetUp()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object, null, null, null, null);

            _loggerMock = new Mock<ILogger<HomeService>>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _contextMock = new Mock<ApplicationDbContext>(options);

            _homeService = new HomeService(_userManagerMock.Object, _roleManagerMock.Object, _contextMock.Object, _loggerMock.Object);
        }

            [Test]
        public async Task AssignRoleAsync_UserIsNull_ReturnsFalse()
        {
            // Arrange
            var fakePrincipal = new ClaimsPrincipal();
            _userManagerMock.Setup(um => um.GetUserAsync(fakePrincipal)).ReturnsAsync((User)null);

            // Act
            var result = await _homeService.AssignRoleAsync(fakePrincipal);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task AssignRoleAsync_UserHasNoRoles_AssignsUserRole_ReturnsTrue()
        {
            // Arrange
            var fakeUser = new User();
            var fakePrincipal = new ClaimsPrincipal();

            _userManagerMock.Setup(um => um.GetUserAsync(fakePrincipal)).ReturnsAsync(fakeUser);
            _userManagerMock.Setup(um => um.GetRolesAsync(fakeUser)).ReturnsAsync(new List<string>());
            _userManagerMock.Setup(um => um.AddToRoleAsync(fakeUser, "User")).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _homeService.AssignRoleAsync(fakePrincipal);

            // Assert
            Assert.IsTrue(result);
            _userManagerMock.Verify(um => um.AddToRoleAsync(fakeUser, "User"), Times.Once);
        }

        [Test]
        public async Task AssignRoleAsync_UserAlreadyHasRole_DoesNotAssignRoleAgain_ReturnsTrue()
        {
            // Arrange
            var fakeUser = new User();
            var fakePrincipal = new ClaimsPrincipal();

            _userManagerMock.Setup(um => um.GetUserAsync(fakePrincipal)).ReturnsAsync(fakeUser);
            _userManagerMock.Setup(um => um.GetRolesAsync(fakeUser)).ReturnsAsync(new List<string> { "User" });

            // Act
            var result = await _homeService.AssignRoleAsync(fakePrincipal);

            // Assert
            Assert.IsTrue(result);
            _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }
    }
}
