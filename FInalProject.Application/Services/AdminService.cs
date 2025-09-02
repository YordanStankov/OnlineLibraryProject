using FInalProject.Application.Interfaces;
using FInalProject.Application.ViewModels.Admin.Book;
using FInalProject.Application.ViewModels.Admin.User;

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
            var reuslt = await _userRepository.BanUserAsync(banId);
            if(reuslt == false)
            {
                return false;
            }
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
            var result = await _userRepository.UnbanUserAsync(unbanId);
            if (result == false)
            {
                return false;
            }
            return true;
        }
    }
}
