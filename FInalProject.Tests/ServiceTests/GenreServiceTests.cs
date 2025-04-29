using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.Services;
using FInalProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FInalProject.Tests.ServiceTests
{
    [TestFixture]
    public class GenreServiceTests
    {
        private ApplicationDbContext _context;
        private UserManager<User> _userManager;
        private GenreService _genreService;

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

            var logger = new LoggerFactory().CreateLogger<GenreService>();
            _genreService = new GenreService(_context, _userManager, logger);
        }

        [TearDown]
        public void TearDown()
        {
            _userManager?.Dispose();
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
        }

        [Test]
        public async Task AddGenreAsync_NewGenre_ReturnsTrue()
        {
            var result = await _genreService.AddGenreAsync("Fantasy");

            Assert.IsTrue(result);
            Assert.AreEqual(1, _context.Genres.Count());
            Assert.AreEqual("Fantasy", _context.Genres.First().Name);
        }

        [Test]
        public async Task AddGenreAsync_ExistingGenre_ReturnsFalse()
        {
            _context.Genres.Add(new Genre { Name = "Sci-Fi" });
            await _context.SaveChangesAsync();

            var result = await _genreService.AddGenreAsync("Sci-Fi");

            Assert.IsFalse(result);
            Assert.AreEqual(1, _context.Genres.Count());
        }

        [Test]
        public async Task DeleteGenreAsync_ExistingGenre_ReturnsTrue()
        {
            var genre = new Genre { Name = "Romance" };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var result = await _genreService.DeleteGenreAsync(genre.Id);

            Assert.IsTrue(result);
            Assert.AreEqual(0, _context.Genres.Count());
        }

        [Test]
        public async Task DeleteGenreAsync_NonExistentGenre_ReturnsFalse()
        {
            var result = await _genreService.DeleteGenreAsync(999);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetGenreListAsync_ReturnsAllGenres()
        {
            _context.Genres.AddRange(
                new Genre { Name = "Adventure" },
                new Genre { Name = "Drama" });
            await _context.SaveChangesAsync();

            var result = await _genreService.GetGenreListAsync();

            Assert.AreEqual(2, result.Count);
            CollectionAssert.AreEquivalent(new[] { "Adventure", "Drama" }, result.Select(g => g.Name));
        }

        [Test]
        public async Task ProvideGenreForPartialAsync_ExistingGenre_ReturnsEditViewModel()
        {
            var genre = new Genre { Name = "Edit Me" };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var result = await _genreService.ProvideGenreForPartialAsync(genre.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(genre.Id, result.Id);
            Assert.AreEqual("Edit Me", result.Name);
        }

        [Test]
        public async Task SaveChangesToGenreAsync_ValidModel_UpdatesGenre()
        {
            var genre = new Genre { Name = "Old Name" };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var model = new GenreEditViewModel
            {
                Id = genre.Id,
                Name = "New Name"
            };

            var result = await _genreService.SaveChangesToGenreAsync(model);

            var updatedGenre = await _context.Genres.FindAsync(genre.Id);
            Assert.IsTrue(result);
            Assert.AreEqual("New Name", updatedGenre.Name);
        }

        [Test]
        public async Task SaveChangesToGenreAsync_InvalidId_ReturnsFalse()
        {
            var model = new GenreEditViewModel
            {
                Id = 999,
                Name = "Doesn't Exist"
            };

            var result = await _genreService.SaveChangesToGenreAsync(model);
            Assert.IsFalse(result);
        }
    }
}
