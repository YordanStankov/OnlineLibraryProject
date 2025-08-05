using FInalProject.Data.Models;
using FInalProject.Application.Services;
using FInalProject.Application.ViewModels.Book.BookOperations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FInalProject.Infrastructure;
namespace FInalProject.Areas.Identity.Pages.Account.Manage
{
    public class BorrowedBooksModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IBookHandlingService _bookHandlingService;
        
        public BorrowedBooksModel (ApplicationDbContext context, UserManager<User> userManager, IBookHandlingService bookHandlingService)
        {
            _context = context;
            _userManager = userManager;
            _bookHandlingService = bookHandlingService;
        }

        public List<BorrowedBook> BorrowedBooksFromUser { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            
            var currUser = await _userManager.GetUserAsync(User);
            if (currUser != null)
            {
                BorrowedBooksFromUser = await _context
                    .BorrowedBooks
                    .Include(b => b.Book)
                    .Where(bb => bb.UserId == currUser.Id)
                    .ToListAsync();
                return Page();
            }
            return NotFound();
        }

        [BindProperty]
        public ReturnBookViewModel model { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            bool response = await _bookHandlingService.ReturnBookAsync(model, User);
            return RedirectToPage();
        }
    }
}
