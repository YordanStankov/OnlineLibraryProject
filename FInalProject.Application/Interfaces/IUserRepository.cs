using FInalProject.Domain.Models;
using FInalProject.Application.ViewModels.Admin.User;
using FInalProject.Application.ViewModels.User.UserOperations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
namespace FInalProject.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(string id);
        Task<PersonalDataViewModel> GetPersonalDataAsync(ClaimsPrincipal user);
        Task<ChangePasswordViewModel> GetChangePasswordAsync(ClaimsPrincipal user);
        Task<IdentityResult> ChangePasswordAsync(ChangePasswordOperationViewModel model);
        Task<bool> UserHasPasswordAsync(ClaimsPrincipal user);
        Task<List<User>> GetAllUsersAsync();
        Task RefreshSignInAsync(string userId);
        Task UpdateUserAsync(User user);
        Task<List<UserListViewModel>> RenderUsersInViewModelAsync();
    }
}
