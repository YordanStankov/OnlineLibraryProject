using FInalProject.Data;
using FInalProject.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace FInalProject.Areas.Identity.Pages.Account.Manage
{
    public class BorrowedBooksModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        

        public BorrowedBooksModel (ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
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
    }
}
