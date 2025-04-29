using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.Services;
using FInalProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FinalProject.Tests.ServiceTests
{
    [TestFixture]
    public class BooksServiceTests
    {
        private ApplicationDbContext _context;
        private UserManager<User> _userManager;
        private BooksService _booksService;
        private ILogger<BooksService> _logger;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            var userStore = new UserStore<User>(_context);
            _userManager = new UserManager<User>(userStore, null, new PasswordHasher<User>(), null, null, null, null, null, null);
            _logger = new LoggerFactory().CreateLogger<BooksService>();
            _booksService = new BooksService(_context, _userManager, _logger);
        }

        [TearDown]
        public void TearDown()
        {
            _userManager?.Dispose();
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllBooksAsync_NoBooks_ReturnsEmptyList()
        {
            var result = await _booksService.GetAllBooksAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task GetAllBooksAsync_WithAvailableBooks_ReturnsBooks()
        {
            var author = new Author { Name = "Author 1" };
            var book = new Book
            {
                Name = "Test Book",
                Pages = 100,
                Author = author,
                AmountInStock = 3,
                Category = Category.Books,
                CategoryString = "Books",
                Description = "test book test book test book",
                CoverImage = "test.jpg",
                BookGenres = new List<BookGenre>
                {
                    new BookGenre { Genre = new Genre { Name = "Drama" } }
                }
            };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var result = await _booksService.GetAllBooksAsync();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Test Book", result[0].Name);
        }

        [Test]
        public async Task CreateBookAsync_ValidData_ReturnsNewBookId()
        {
            var model = new BookCreationViewModel
            {
                Name = "New Book",
                AuthorName = "New Author",
                AmountInStock = 5,
                Pages = 120,
                ReadingTime = 60,
                Category = Category.Education,
                CoverImage = "image.jpg",
                Description = "Description here",
                SelectedGenreIds = new List<int>()
            };

            var id = await _booksService.CreateBookAsync(model);

            var book = await _context.Books.FindAsync(id);
            Assert.IsNotNull(book);
            Assert.AreEqual("New Book", book.Name);
            Assert.AreEqual("New Author", book.Author.Name);
        }

        [Test]
        public async Task CreateBookAsync_WithExistingAuthor_ReusesAuthor()
        {
            var existingAuthor = new Author { Name = "Known Author" };
            _context.Authors.Add(existingAuthor);
            await _context.SaveChangesAsync();

            var model = new BookCreationViewModel
            {
                Name = "Second Book",
                AuthorName = "Known Author",
                AmountInStock = 2,
                Pages = 90,
                ReadingTime = 45,
                Category = Category.Magazine,
                CoverImage = "img.jpg",
                Description = "Another description"
            };

            var id = await _booksService.CreateBookAsync(model);

            var book = await _context.Books.FindAsync(id);
            Assert.AreEqual(existingAuthor.Id, book.AuthorId);
        }

       
    }
}
