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
using System.Security.Claims;
using System.Threading.Tasks;

namespace FinalProject.Tests.ServiceTests
{
    [TestFixture]
    public class BookOperationsServiceTests
    {
        private ApplicationDbContext _context;
        private UserManager<User> _userManager;
        private BookOprationsService _bookService;
        private ILogger<BookOprationsService> _logger;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            _context = new ApplicationDbContext(options);

            var userStore = new UserStore<User>(_context);
            _userManager = new UserManager<User>(userStore, null, new PasswordHasher<User>(), null, null, null, null, null, null);

            _logger = new LoggerFactory().CreateLogger<BookOprationsService>();
            _bookService = new BookOprationsService(_context, _userManager, _logger);
        }

        [TearDown]
        public void TearDown()
        {
            _userManager?.Dispose();
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task BorrowBookAsync_ValidUserAndBook_Succeeds()
        {
            var user = new User { UserName = "borrower", Email = "borrower@example.com" };
            await _userManager.CreateAsync(user, "Password123!");
            var book = new Book
            {
                Name = "Book for comment",
                AmountInStock = 5,
                CategoryString = "Educational",
                CoverImage = "cover.jpg",
                Description = "A great book",
                Pages = 100,
                ReadingTime = 60
            };
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            var claims = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id) });
            var principal = new ClaimsPrincipal(claims);

            var result = await _bookService.BorrowBookAsync(book.Id, principal);

            Assert.IsTrue(result);
            Assert.AreEqual(4, (await _context.Books.FindAsync(book.Id)).AmountInStock);
        }

        [Test]
        public async Task CreateCommentAsync_ValidData_ReturnsBookId()
        {
            var user = new User { UserName = "commenter", Email = "comment@example.com" };
            await _userManager.CreateAsync(user, "Password123!");
            var book = new Book
            {
                Name = "Book for comment",
                AmountInStock = 5,
                CategoryString = "Educational",
                CoverImage = "cover.jpg",
                Description = "A great book",
                Pages = 100,
                ReadingTime = 60
            };
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            var claims = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id) });
            var principal = new ClaimsPrincipal(claims);

            var model = new CreateCommentViewModel
            {
                BookId = book.Id,
                Description = "Great book!"
            };

            var result = await _bookService.CreateCommentAsync(model, principal);

            Assert.AreEqual(book.Id, result);
            Assert.AreEqual(1, await _context.Comments.CountAsync());
        }

        [Test]
        public async Task DeleteBookAsync_ExistingBook_DeletesBook()
        {
            var book = new Book
            {
                Name = "Book for comment",
                AmountInStock = 5,
                CategoryString = "Educational",
                CoverImage = "cover.jpg",
                Description = "A great book",
                Pages = 100,
                ReadingTime = 60
                , Comments = new List<Comment>()
                , BookGenres = new List<BookGenre>()
                , Favourites = new List<Favourite>() };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var result = await _bookService.DeleteBookAsync(book.Id);

            Assert.IsTrue(result);
            Assert.AreEqual(0, await _context.Books.CountAsync());
        }

        [Test]
        public async Task ReturnSearchResultsAync_ValidString_ReturnsResults()
        {
            var author = new Author { Name = "Test Author" };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            var book = new Book
            {
                Name = "Interesting Book",
                AmountInStock = 5,
                CategoryString = "Educational",
                CoverImage = "cover.jpg",
                Description = "A great book",
                Pages = 100,
                ReadingTime = 60, 
                Author = author,
                BookGenres = new List<BookGenre>() };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var results = await _bookService.ReturnSearchResultsAync("Test");

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Interesting Book", results[0].Name);
        }
    }
}
