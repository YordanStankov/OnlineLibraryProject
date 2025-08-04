using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.Application.Interfaces;
using FInalProject.Application.ViewModels.Admin.User;
using Microsoft.EntityFrameworkCore;

namespace FInalProject.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        
        public UserRepository (ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<UserListViewModel>> RenderUsersInViewModelAsync()
        {
            return await _context.Users.Select(u => new UserListViewModel
            {
                UserId = u.Id,
                UserName = u.UserName,
                Strikes = u.Strikes ?? 0,
                CantBorrow = u.CantBorrow
            }).ToListAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
             _context.Users.Update(user);
            await _context.SaveChangesAsync();  
        }
    }
}
