using Microsoft.AspNetCore.Mvc;
using FInalProject.Data.Models;
using FInalProject.ViewModels;
using FInalProject.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using FInalProject.Services;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace FInalProject.Controllers
{
    public class BookOperationsController : Controller
    {
        private readonly IBookOprationsService _bookOprationsService;

        public BookOperationsController(IBookOprationsService bookOprationsService)
        {
            _bookOprationsService = bookOprationsService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteBook(int doomedId)
        {
            bool succes = await _bookOprationsService.DeleteBookAsync(doomedId);
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
            int response = await _bookOprationsService.CreateCommentAsync(comment, User);
            if(response == -1)
            {
                return Unauthorized();
            }
            return RedirectToAction("BookFocus", "Books", new {Id = response}); 
        }
        [HttpGet]
        public async Task<IActionResult> Search(string searchedString)
        {
            
            var books = await _bookOprationsService.ReturnSearchResultsAync(searchedString);
            TempData["Books"] = JsonConvert.SerializeObject(books);
            return RedirectToAction("SearchResults", "Books");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookCreationViewModel model)
        {
            var result = await _bookOprationsService.EditBookAsync(model);
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
            var response = await _bookOprationsService.BorrowBookAsync(borrowedId, User);
            if(response == true)
            {
                return Json(new { succes = true, redirectUrl = Url.Action("AllBooks", "Books") });
            }
            return Json(new { succes = false, message = "Book not found" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rating (int amount, int bookId)
        {
            var response = await _bookOprationsService.UpdateFavouritesAsync(amount, bookId, User);
            return RedirectToAction("BookFocus", "Books", new { id = bookId });
        }
    }
}
