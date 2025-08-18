using FInalProject.Application.ViewModels.Admin.User;
using FInalProject.Application.ViewModels.User.UserOperations;
using FInalProject.Domain.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
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
        Task<string?> GetPasswordHashAsync(string userId, CancellationToken cancellationToken);
        Task<IList<UserLoginInfo>> GetLoginsAsync(string userId);
        Task<IList<AuthenticationScheme>> GetOtherLoginsAsync(IList<UserLoginInfo> userLoginInfo);
        Task UpdateUserAsync(User user);
        Task<List<UserListViewModel>> RenderUsersInViewModelAsync();
    }
}
