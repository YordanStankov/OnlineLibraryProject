using FInalProject.Data.Models;
using FInalProject.ViewModels;
namespace FInalProject.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(string id);
        Task<List<User>> GetAllUsersAsync();
        Task UpdateUserAsync(User user);
        Task<List<UserListViewModel>> RenderUsersInViewModelAsync();
    }
}
