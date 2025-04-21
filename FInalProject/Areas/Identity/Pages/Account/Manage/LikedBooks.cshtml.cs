using FInalProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FInalProject.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FInalProject.Areas.Identity.Pages.Account.Manage
{
    public class LikedBooksModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public LikedBooksModel(ApplicationDbContext dbContext, UserManager<User> userManager)
        {
            _context = dbContext;
            _userManager = userManager;
        }

        public List<Book> FavouritesOfUser { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var currUserId = _userManager.GetUserId(User);
            
                FavouritesOfUser = await _context.Books
                .Include(b => b.Favourites)
                .Where(b => b.Favourites.Any(f => f.UserId == currUserId && f.Amount > 0))
                .ToListAsync();
                return Page();
        }
    }
}
