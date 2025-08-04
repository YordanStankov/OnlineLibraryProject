using FInalProject.Data.Models;
using FInalProject.Application.ViewModels.Admin.User;
namespace FInalProject.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(string id);
        Task<List<User>> GetAllUsersAsync();
        Task UpdateUserAsync(User user);
        Task<List<UserListViewModel>> RenderUsersInViewModelAsync();
    }
}
