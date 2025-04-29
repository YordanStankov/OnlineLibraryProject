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
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FInalProject.Tests.ControllerTests
{
    [TestFixture]
    public class BooksControllerTests
    {
        private ApplicationDbContext _context;
        private UserManager<User> _userManager;
        private IBooksService _booksService;
        private Mock<IBookOprationsService> _bookOperationsMock;
        private BooksController _controller;

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

            var logger = new LoggerFactory().CreateLogger<BooksService>();
            _booksService = new BooksService(_context, _userManager, logger);
            _bookOperationsMock = new Mock<IBookOprationsService>();

            _controller = new BooksController(_booksService, _bookOperationsMock.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
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
        public async Task AllBooks_ReturnsFilteredList()
        {
            _context.Books.Add(new Book {
                Name = "New Book",
                AmountInStock = 5,
                Pages = 100,
                ReadingTime = 60,
                Category = Category.Education,
                CategoryString = "Education",
                CoverImage = "img.jpg",
                Description = "Testing"
            });
            await _context.SaveChangesAsync();

            _bookOperationsMock.Setup(op => op.ApplyFiltering(It.IsAny<List<BookListViewModel>>(), It.IsAny<FilteringViewModel>()))
                .ReturnsAsync((List<BookListViewModel> books, FilteringViewModel _) => books);

            var result = await _controller.AllBooks(new FilteringViewModel()) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<BookListViewModel>>(result.Model);
        }

        [Test]
        public async Task BooksFromSpecificCategory_ReturnsFilteredCategoryView()
        {
            var book = new Book {
                Name = "New Book",
                AmountInStock = 5,
                Pages = 100,
                ReadingTime = 60,
                Category = Category.Education,
                CategoryString = "Education",
                CoverImage = "img.jpg",
                Description = "Testing"
            };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var categoryResult = await _booksService.GetAllBooksFromSpecificCategoryAsync((int)Category.Books);
            _bookOperationsMock.Setup(x => x.ApplyFiltering(It.IsAny<IEnumerable<BookListViewModel>>(), It.IsAny<FilteringViewModel>()))
                .ReturnsAsync(categoryResult.BooksFromCategory.ToList());

            var result = await _controller.BooksFromSpecificCategory((int)Category.Books, new FilteringViewModel()) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BooksFromCategoryViewModel>(result.Model);
        }

        [Test]
        public async Task BookCreation_ReturnsFormModel()
        {
            var result = await _controller.BookCreation() as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BookCreationViewModel>(result.Model);
        }

        [Test]
        public async Task CreateABook_Valid_RedirectsToBookFocus()
        {
            var model = new BookCreationViewModel
            {
                Name = "New Book",
                AuthorName = "Author",
                AmountInStock = 5,
                Pages = 100,
                ReadingTime = 60,
                Category = Category.Education,
                CoverImage = "img.jpg",
                Description = "Testing"
            };

            var result = await _controller.CreateABook(model) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("BookFocus", result.ActionName);
        }
    }
}
