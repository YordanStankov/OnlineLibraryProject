using Microsoft.AspNetCore.Mvc;
using FInalProject.Models;
using FInalProject.ViewModels;
using FInalProject.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using FInalProject.Services; 

namespace FInalProject.Controllers
{
    public class BookOperationsController : Controller
    {
        private readonly IBookOprationsService _bookOprationsService;

        public BookOperationsController(IBookOprationsService bookOprationsService)
        {
            _bookOprationsService = bookOprationsService;
        }

        [HttpDelete]
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
        public async Task<IActionResult> CreateComment(CreateCommentViewModel comment)
        {
            int response = await _bookOprationsService.CreateCommentAsync(comment, User);
            if(response == -1)
            {
                return Unauthorized();
            }
            return RedirectToAction("BookFocus", "Books", new {Id = response}); 
        }
        
    }
}
