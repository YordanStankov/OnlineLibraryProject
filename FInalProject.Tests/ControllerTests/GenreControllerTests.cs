using FInalProject.Controllers;
using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.Services;
using FInalProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FInalProject.Tests.ControllerTests
{
    [TestFixture]
    public class GenreControllerTests
    {
        private ApplicationDbContext _context;
        private UserManager<User> _userManager;
        private GenreService _genreService;
        private Mock<IBookOprationsService> _bookOperationsMock;
        private GenreController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            var userStore = new UserStore<User>(_context);
            _userManager = new UserManager<User>(
                userStore, null, new PasswordHasher<User>(), null, null, null, null, null, null);

            var logger = new LoggerFactory().CreateLogger<GenreService>();

            _genreService = new GenreService(_context, _userManager, logger);
            _bookOperationsMock = new Mock<IBookOprationsService>();
            _controller = new GenreController(_genreService, _bookOperationsMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
            _userManager?.Dispose();
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
        }

        [Test]
        public async Task GenreList_ReturnsAllGenres()
        {
            _context.Genres.Add(new Genre { Name = "Drama" });
            await _context.SaveChangesAsync();

            var result = await _controller.GenreList() as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<Genre>>(result.Model);
            var model = result.Model as List<Genre>;
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("Drama", model[0].Name);
        }

        [Test]
        public async Task AddGenre_Valid_AddsAndRedirects()
        {
            var result = await _controller.AddGenre("Mystery") as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("GenreList", result.ActionName);
            Assert.AreEqual(1, await _context.Genres.CountAsync());
        }

        [Test]
        public void AddGenre_Existing_Throws()
        {
            _context.Genres.Add(new Genre { Name = "Existing" });
            _context.SaveChanges();

            Assert.ThrowsAsync<ArgumentException>(() => _controller.AddGenre("Existing"));
        }

        [Test]
        public async Task DeleteGenre_Existing_Redirects()
        {
            var genre = new Genre { Name = "ToDelete" };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteGenre(genre.Id) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("GenreList", result.ActionName);
            Assert.AreEqual(0, _context.Genres.Count());
        }

        [Test]
        public void DeleteGenre_NonExistent_Throws()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _controller.DeleteGenre(999));
        }

        [Test]
        public async Task EditGenre_ReturnsPartialWithViewModel()
        {
            var genre = new Genre { Name = "EditThis" };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var result = await _controller.EditGenre(genre.Id) as PartialViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("_EditGenre", result.ViewName);
            Assert.IsInstanceOf<GenreEditViewModel>(result.Model);
        }

        [Test]
        public async Task SaveChangesToGenre_Valid_UpdatesGenre()
        {
            var genre = new Genre { Name = "Old" };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var model = new GenreEditViewModel { Id = genre.Id, Name = "Updated" };

            var result = await _controller.SaveChangesToGenre(model) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("GenreList", result.ActionName);
            Assert.AreEqual("Updated", _context.Genres.First().Name);
        }
    }
}
