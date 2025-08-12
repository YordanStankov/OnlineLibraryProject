using Microsoft.AspNetCore.Mvc;
using FInalProject.Application.Services;
using FInalProject.Application.ViewModels.Book;
using FInalProject.Application.ViewModels.Comment.CommentOperations;

namespace FInalProject.Web.Controllers
{
    public class BookOperationsController : Controller
    {
        private readonly IBookCRUDService _bookCRUDService;
        private readonly IBookHandlingService _bookHandlingService;
        private readonly ICommentService _commentService;
        private readonly IFavouriteService _favouriteService;

        public BookOperationsController( IBookCRUDService bookCRUDService, IBookHandlingService bookHandlingService, ICommentService commentService, IFavouriteService favouriteService)
        {
            _bookCRUDService = bookCRUDService;
            _bookHandlingService = bookHandlingService;
            _commentService = commentService;
            _favouriteService = favouriteService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteBook(int doomedId)
        {
            bool succes = await _bookCRUDService.DeleteBookAsync(doomedId);
            if(succes == true)
            {
                return Json(new { succes = true, redirectUrl = Url.Action("AllBooks", "Books") });
            }
            return Json(new { succes = false, message = "Book not found" }); 

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment(CreateCommentViewModel comment)
        {
            int response = await _commentService.CreateCommentAsync(comment, User);
            if(response == -1)
            {
                return Unauthorized();
            }
            return RedirectToAction("BookFocus", "Books", new {Id = response}); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookCreationViewModel model)
        {
            var result = await _bookCRUDService.EditBookAsync(model);
            if(result != true)
            {
                ModelState.AddModelError("", "Failed to update book");
            }
                return RedirectToAction("AllBooks", "Books");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BorrowBook(int borrowedId)
        {
            var response = await _bookHandlingService.BorrowBookAsync(borrowedId, User);
            if(response == true)
            {
                return RedirectToAction("BookFocus", "Books", new { id = borrowedId });
            }
                return RedirectToAction("AllBooks", "Books");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rating (int amount, int bookId)
        {
            await _favouriteService.UpdateFavouritesAsync(amount, bookId, User);
            return RedirectToAction("BookFocus", "Books", new { id = bookId });
        }
    }
}
