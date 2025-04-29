using FInalProject.Controllers;
using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.Services;
using FInalProject.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace FInalProject.Tests.ControllerTests
{
    [TestFixture]
    public class AuthorControllerTests
    {
        private ApplicationDbContext _context;
        private UserManager<User> _userManager;
        private AuthorsService _authorsService;
        private AuthorsController _authorsController;

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

            _authorsService = new AuthorsService(_context, _userManager);
            _authorsController = new AuthorsController(_authorsService, _userManager);
            _authorsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };


            var mockTempDataProvider = new Mock<ITempDataProvider>();

            _authorsController.TempData = new TempDataDictionary(
                _authorsController.ControllerContext.HttpContext,
                mockTempDataProvider.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _authorsController?.Dispose();
            _userManager?.Dispose();
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
        }

        [Test]
        public async Task AllAuthors_ReturnsViewWithAuthors()
        {
            _context.Authors.Add(new Author { Name = "Author One" });
            await _context.SaveChangesAsync();

            var result = await _authorsController.AllAuthors() as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<AuthorListViewModel>>(result.Model);
            var model = result.Model as List<AuthorListViewModel>;
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("Author One", model[0].AuthorName);
        }


        [Test]
        public async Task AuthorProfile_ExistingAuthor_ReturnsViewWithProfile()
        {
            var author = new Author { Name = "Profile Author" };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            
            var fakeUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }, "mock"));
            _authorsController.ControllerContext.HttpContext.User = fakeUser;

            var result = await _authorsController.AuthorProfile(author.Id) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<AuthorProfileViewModel>(result.Model);
            var model = result.Model as AuthorProfileViewModel;
            Assert.AreEqual("Profile Author", model.AuthorName);
        }

        [Test]
        public async Task AddAuthorPortrait_ValidModel_UpdatesPortraitAndRedirects()
        {
            var author = new Author { Name = "Portrait Author" };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            var model = new AddAuthorPortraitViewModel
            {
                Id = author.Id,
                Picture = "portrait.jpg"
            };

            var result = await _authorsController.AddAuthorPortrait(model) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("AuthorProfile", result.ActionName);
            Assert.AreEqual(author.Id, result.RouteValues["authorId"]);

            var updatedAuthor = await _context.Authors.FindAsync(author.Id);
            Assert.AreEqual("portrait.jpg", updatedAuthor.Portrait);
        }

        [Test]
        public async Task AuthorSearchResults_MatchingName_ReturnsFilteredList()
        {
            _context.Authors.Add(new Author { Name = "Jane Austen" });
            _context.Authors.Add(new Author { Name = "J. K. Rowling" });
            await _context.SaveChangesAsync();

            var result = await _authorsController.AuthorSearchResults("Jane") as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<AuthorSearchResultsViewModel>(result.Model);

            var model = result.Model as AuthorSearchResultsViewModel;

            Assert.IsNotNull(model.authorsFound);
            Assert.AreEqual(1, model.authorsFound.Count);
        }


        [Test]
        public async Task AuthorSearchResults_NoMatch_ReturnsEmptyList()
        {
            var result = await _authorsController.AuthorSearchResults("Unknown") as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<AuthorSearchResultsViewModel>(result.Model);

            var model = result.Model as AuthorSearchResultsViewModel;
            Assert.IsNotNull(model.authorsFound);
            Assert.AreEqual(0, model.authorsFound.Count);
        }

    }
}
