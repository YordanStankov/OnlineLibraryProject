using Microsoft.AspNetCore.Identity;
using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.ViewModels;
using Microsoft.EntityFrameworkCore;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace FInalProject.Services
{
    public interface IAdminService
    {
        Task<bool> BanUser(string banId);
        Task<bool> UnbanUser(string unbanId);
        Task<List<UserListViewModel>> RenderUserListAsync();
        Task<List<AdminBookListViewModel>> RenderBookListAsync();
    }
    public class AdminService : IAdminService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        public AdminService(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<bool> BanUser(string banId)
        {
            var User = await _context.Users.FirstOrDefaultAsync(u => u.Id == banId);
            if(User == null)
            {
                return false;
            }
            User.CantBorrow = true;
            User.Strikes = 3;
            _context.Update(User);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<AdminBookListViewModel>> RenderBookListAsync()
        {
            return await _context.Books.AsNoTracking().Select(b => new AdminBookListViewModel
            {
                BookId = b.Id,
                BookName = b.Name,
                BooksBorrowed = b.BorrowedBooks.Count(),
                BookStock = b.AmountInStock,
                Category = b.CategoryString,
                genres = b.BookGenres.Select(bg => bg.Genre.Name).ToList() ?? new List<string>()
            }).ToListAsync();
        }

        public async Task<List<UserListViewModel>> RenderUserListAsync()
        {
            return await _context.Users.AsNoTracking().Select(u => new UserListViewModel
            {
                UserId = u.Id,
                UserName = u.UserName,
                Strikes = u.Strikes ?? 0,
                CantBorrow = u.CantBorrow
            }).ToListAsync();
        }

        public async Task<bool> UnbanUser(string unbanId)
        {
            var User = await _context.Users.FirstOrDefaultAsync(u => u.Id == unbanId);
            if (User == null)
            {
                return false;
            }
            User.CantBorrow = false;
            User.Strikes = 0;
            _context.Update(User);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
