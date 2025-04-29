using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.Services;
using FInalProject.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace FInalProject.Tests.ServiceTests
{
    [TestFixture]
    public class AuthorsServiceTests
    {
        private ApplicationDbContext _context;
        private UserManager<User> _userManager;
        private AuthorsService _authorsService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            var userStore = new UserStore<User>(_context);
            _userManager = new UserManager<User>(
                userStore,
                null,
                new PasswordHasher<User>(),
                null, null, null, null, null, null);

            _authorsService = new AuthorsService(_context, _userManager);
        }

        [TearDown]
        public void TearDown()
        {
            _userManager?.Dispose();
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
        }

        [Test]
        public async Task RenderAuthorListAsync_NoAuthors_ReturnsEmptyList()
        {
            var result = await _authorsService.RenderAuthorListAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task RenderAuthorListAsync_WithAuthors_ReturnsAuthorList()
        {
            _context.Authors.Add(new Author { Name = "Test Author" });
            await _context.SaveChangesAsync();

            var result = await _authorsService.RenderAuthorListAsync();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Test Author", result[0].AuthorName);
        }

        [Test]
        public async Task RenderAuthorProfileAsync_ExistingAuthor_ReturnsProfile()
        {
            var author = new Author { Name = "Profile Author" };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }));

            var result = await _authorsService.RenderAuthorProfileAsync(author.Id, claimsPrincipal);

            Assert.IsNotNull(result);
            Assert.AreEqual("Profile Author", result.AuthorName);
        }


        [Test]
        public async Task RenderSearchResultsAsync_MatchingAuthors_ReturnsResults()
        {
            _context.Authors.Add(new Author { Name = "Searching Name" });
            await _context.SaveChangesAsync();

            var result = await _authorsService.RenderSearchResultsAsync("Searching");

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.authorsFound.Count);
        }

        [Test]
        public async Task RenderSearchResultsAsync_NoMatches_ReturnsEmptyList()
        {
            var result = await _authorsService.RenderSearchResultsAsync("NothingFound");

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.authorsFound.Count);
        }

        [Test]
        public async Task AddPortraitToAuthorAsync_ExistingAuthor_UpdatesPortrait()
        {
            var author = new Author { Name = "Portrait Author" };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            var model = new AddAuthorPortraitViewModel
            {
                Id = author.Id,
                Picture = "portrait.jpg"
            };

            await _authorsService.AddPortraitToAuthorAsync(model);

            var updatedAuthor = await _context.Authors.FindAsync(author.Id);
            Assert.AreEqual("portrait.jpg", updatedAuthor.Portrait);
        }

        [Test]
        public async Task FavouriteAuthorAsync_AddsFavourite()
        {
            var user = new User { UserName = "favoriteuser", Email = "favorite@example.com" };
            await _userManager.CreateAsync(user, "Test123!");

            var author = new Author { Name = "Favourite Author" };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            }));

            await _authorsService.FavouriteAuthorAsync(author.Id, claimsPrincipal);

            var favourite = _context.FavouriteAuthors.FirstOrDefault(f => f.UserId == user.Id && f.AuthorId == author.Id);
            Assert.IsNotNull(favourite);
        }
    }
}
