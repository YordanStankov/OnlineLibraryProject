using FInalProject.Controllers;
using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.Services;
using FInalProject.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace FInalProject.Tests.ControllerTests
{
    [TestFixture]
    public class AdminControllerTests
    {
        private ApplicationDbContext _context;
        private UserManager<User> _userManager;
        private AdminService _adminService;
        private AdminController _adminController;

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
                null,
                new PasswordHasher<User>(),
                null, null, null, null, null, null);

            _adminService = new AdminService(_userManager, _context);
            _adminController = new AdminController(_adminService);

            _adminController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ControllerActionDescriptor()
            };

            _adminController.Url = new UrlHelper(_adminController.ControllerContext);
        }

        [TearDown]
        public void TearDown()
        {
            _adminController?.Dispose();
            _userManager?.Dispose();
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
        }

        [Test]
        public async Task AdminPanel_ShouldReturnPopulatedViewModel()
        {
            // Act
            var result = await _adminController.AdminPanel(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<AdminPanelViewModel>(result.Model);

            var model = result.Model as AdminPanelViewModel;
            Assert.AreEqual(1, model.BookOrUser);
            Assert.IsNotNull(model.BookList);
            Assert.IsNotNull(model.UserList);
        }

        [Test]
        public async Task AdminPanelSetting_ShouldRedirectWithCorrectSetting()
        {
            // Act
            var result = await _adminController.AdminPanelSetting(0) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AdminPanel", result.ActionName);
            Assert.AreEqual(0, result.RouteValues["setting"]);
        }

       
        [Test]
        public async Task BanUser_ShouldReturnFailure_WhenUserDoesNotExist()
        {
            // Act
            var result = await _adminController.BanUser("non-existent-id") as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            var data = result.Value.GetType()
                .GetProperties()
                .ToDictionary(p => p.Name, p => p.GetValue(result.Value));

            Assert.AreEqual(false, data["succes"]);
            Assert.AreEqual("User not found", data["message"]);
        }
    }
}
