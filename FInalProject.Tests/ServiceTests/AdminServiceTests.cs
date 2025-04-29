using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace FInalProject.Tests.ServiceTests
{
    [TestFixture]
    public class AdminServiceTests
    {
        private ApplicationDbContext _context;
        private UserManager<User> _userManager;
        private AdminService _adminService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            var userStore = new UserStore<User>(_context);
            _userManager = new UserManager<User>(
                userStore,
                null, // IOptions<IdentityOptions>
                new PasswordHasher<User>(),
                null, null, null, null, null, null);

            _adminService = new AdminService(_userManager, _context);
        }

        [TearDown]
        public void TearDown()
        {
            _userManager?.Dispose();
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task BanUser_ShouldSetCantBorrowTrue_WhenUserExists()
        {
            // Arrange
            var user = new User { UserName = "admin", Email = "admin@example.com" };
            await _userManager.CreateAsync(user, "Test123!");

            // Act
            var result = await _adminService.BanUser(user.Id);

            // Assert
            Assert.IsTrue(result, "Expected BanUser to return true for an existing user.");

            var updatedUser = await _userManager.FindByIdAsync(user.Id);
            Assert.IsNotNull(updatedUser);
            Assert.IsTrue(updatedUser.CantBorrow, "Expected CantBorrow flag to be true after banning.");
        }
    }
}
