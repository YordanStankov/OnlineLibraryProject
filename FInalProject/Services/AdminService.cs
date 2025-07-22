using FInalProject.Data.Models;
using FInalProject.ViewModels;
using FInalProject.Repositories.Interfaces;


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
            return await _bookRepository.RenderAdminBookListAsync();
        }

        public async Task<List<UserListViewModel>> RenderUserListAsync()
        {
            return await _userRepository.RenderUsersInViewModelAsync();
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
