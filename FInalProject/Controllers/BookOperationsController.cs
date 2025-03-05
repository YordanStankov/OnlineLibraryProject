using Microsoft.AspNetCore.Mvc;
using FInalProject.Models;
using FInalProject.ViewModels;
using FInalProject.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace FInalProject.Controllers
{
    public class BookOperationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        
        public BookOperationsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager; 
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBook(int doomedId)
        {
            var doomedBook = await _context.Books
                .Include(b => b.Comments)
                .Include(b => b.Favourites)
                .Include(b => b.BookGenres)
                .FirstOrDefaultAsync(b => b.Id == doomedId);
            if(doomedBook is not null)
            {
                _context.Remove(doomedBook); 
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("AllBooks", "Books");
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(CreateCommentViewModel comment)
        {
            Comment commentFloat = new Comment()
            {
                UserId = _userManager.GetUserId(User),
                BookId = comment.BookId,
                CommentContent = comment.Description
            };
            _context.Add(commentFloat);
            _context.SaveChanges(); 
            return RedirectToAction("BookFocus", "Books", new {Id = commentFloat.BookId}); 
        }
        
    }
}
