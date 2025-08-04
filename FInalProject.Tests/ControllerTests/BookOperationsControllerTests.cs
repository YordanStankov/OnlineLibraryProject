using FInalProject.Controllers;
using FInalProject.Application.Services;
using FInalProject.Application.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FInalProject.Tests.ControllerTests
{
    [TestFixture]
    public class BookOperationsControllerTests
    {
        private Mock<IBookOprationsService> _bookOpsMock;
        private BookOperationsController _controller;

        [SetUp]
        public void Setup()
        {
            _bookOpsMock = new Mock<IBookOprationsService>();
            _controller = new BookOperationsController(_bookOpsMock.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }
        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }
        [Test]
        public async Task CreateComment_ValidComment_RedirectsToBookFocus()
        {
            var comment = new CreateCommentViewModel
            {
                BookId = 1,
                Description = "Great book!"
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user123")
            }, "mock"));

            _controller.ControllerContext.HttpContext.User = user;

            _bookOpsMock.Setup(x => x.CreateCommentAsync(comment, user)).ReturnsAsync(1);

            var result = await _controller.CreateComment(comment) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("BookFocus", result.ActionName);
            Assert.AreEqual("Books", result.ControllerName);
            Assert.AreEqual(1, result.RouteValues["Id"]);
        }

        [Test]
        public async Task Edit_ValidModel_RedirectsToAllBooks()
        {
            var model = new BookCreationViewModel
            {
                Name = "Updated Book",
                AuthorName = "Author",
                Description = "Updated",
                CoverImage = "image.jpg",
                Pages = 100,
                ReadingTime = 45,
                AmountInStock = 3,
                Category = Data.Models.Category.Education
            };

            _bookOpsMock.Setup(x => x.EditBookAsync(model)).ReturnsAsync(true);

            var result = await _controller.Edit(model) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("AllBooks", result.ActionName);
            Assert.AreEqual("Books", result.ControllerName);
        }

        [Test]
        public async Task Rating_ValidInput_RedirectsToBookFocus()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user456")
            }, "mock"));
            _controller.ControllerContext.HttpContext.User = user;

            _bookOpsMock.Setup(x => x.UpdateFavouritesAsync(5, 1, user)).ReturnsAsync(true);

            var result = await _controller.Rating(5, 1) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("BookFocus", result.ActionName);
            Assert.AreEqual("Books", result.ControllerName);
            Assert.AreEqual(1, result.RouteValues["id"]);
        }
    }
}
