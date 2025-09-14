using FInalProject.Application.Interfaces;
using FInalProject.Application.ViewModels.Admin.Book;
using FInalProject.Application.ViewModels.Admin.User;
using FInalProject.Domain.Models;

namespace FInalProject.Application.Services
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
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;
        public AdminService( IUserRepository userRepository, IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> BanUser(string banId)
        {
            var user = await _userRepository.GetUserByIdAsync(banId);
            if(user == null)
            {
                return false;
            }
            user.CantBorrow = true;
            user.Strikes = 3;
            await _userRepository.UpdateUserAsync(user);
            return true;
        }

        public async Task<List<AdminBookListViewModel>> RenderBookListAsync()
        {
            var books = await _bookRepository.GetAllAdminBooksDTOAsync();
            return books.Select(b => new AdminBookListViewModel
            {   
                BookId = b.BookId,
                BookName = b.BookName,
                BooksBorrowed = b.BooksBorrowed,
                BookStock = b.BookStock,
                Category = b.Category,
                genres = b.genres
            }).ToList();
        }

        public async Task<List<UserListViewModel>> RenderUserListAsync()
        {
            var users = await _userRepository.RenderUsersInViewModelAsync();
            return users.Select(u => new UserListViewModel
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Strikes = u.Strikes,
                CantBorrow = u.CantBorrow
            }).ToList();
        }


        public async Task<bool> UnbanUser(string unbanId)
        {
            var User = await _userRepository.GetUserByIdAsync(unbanId);
            if (User == null)
            {
                return false;
            }
            User.CantBorrow = false;
            User.Strikes = 0;
            await _userRepository.UpdateUserAsync(User);
            return true;
        }
    }
}
