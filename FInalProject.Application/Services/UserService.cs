
using FInalProject.Application.Interfaces;
using FInalProject.Application.ViewModels.User.UserOperations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FInalProject.Application.Services
{
    public interface IUserService
    {
        Task<PersonalDataViewModel> GetPersonalDataAsync(ClaimsPrincipal user);
        Task<ChangePasswordViewModel> GetChangePasswordAsync(ClaimsPrincipal user);
        Task<bool> UserHasPasswordAsync(ClaimsPrincipal user);
        Task RefreshSignInAsync(string userId);
        Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string userId);
        Task<IdentityResult> RemoveLoginAsync(ExternalLoginsViewModel model);
        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, ClaimsPrincipal user);
        Task<ExternalLoginsViewModel> GetExternalLoginsAsync(ClaimsPrincipal user, string loginProvider, string providerKey);
        Task<IdentityResult> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
        Task<IdentityResult> AddLoginAsync(ClaimsPrincipal user, ExternalLoginInfo loginInfo);
        Task<IList<UserLoginInfo>> GetLoginsAsync(string userId);
        Task<bool> CanRemoveLoginAsync(string userId, int currentLoginsCount, CancellationToken cancellationToken);
        Task<IList<AuthenticationScheme>> GetOtherLoginsAsync(IList<UserLoginInfo> userLoginInfo);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository) 
        { 
            _userRepository = userRepository; 
        }

        public async Task<IdentityResult> AddLoginAsync(ClaimsPrincipal user, ExternalLoginInfo loginInfo)
        {
            IdentityResult result = await _userRepository.AddLoginAsync(user, loginInfo);
            return result;
        }

        public async Task<bool> CanRemoveLoginAsync(string userId, int currentLoginsCount, CancellationToken cancellationToken)
        {
            var passwordHash = await _userRepository.GetPasswordHashAsync(userId, cancellationToken);
            return passwordHash != null || currentLoginsCount > 1;
        }

       
        public async Task<IdentityResult> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            ChangePasswordOperationViewModel model = new ChangePasswordOperationViewModel
            {
                UserId = userId,
                OldPassword = oldPassword,
                 NewPassword = newPassword
            };
            var result = await _userRepository.ChangePasswordAsync(model);
            return result;
        }

        public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, ClaimsPrincipal user)
        {
            AuthenticationProperties properites = _userRepository.ConfigureExternalAuthenticationProperties(provider, redirectUrl, user);
            return properites;
        }

        public async Task<ChangePasswordViewModel> GetChangePasswordAsync(ClaimsPrincipal user)
        {
            var CurrUser = await _userRepository.GetChangePasswordAsync(user);
            if (CurrUser == null)
                throw new Exception("User is null in GetChangePasswordASync method in UserService");
            return CurrUser;
        }

        public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string userId)
        {
            ExternalLoginInfo info = await _userRepository.GetExternalLoginInfoAsync(userId);
            return info;
        }

        public async Task<ExternalLoginsViewModel> GetExternalLoginsAsync(ClaimsPrincipal user, string loginProvider, string providerKey)
        {
            var model = await _userRepository.GetExternalLoginsAsync(user, loginProvider, providerKey);
            return model;
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(string userId)
        {
            var list = await _userRepository.GetLoginsAsync(userId);
            return list;
        }

        public async Task<IList<AuthenticationScheme>> GetOtherLoginsAsync(IList<UserLoginInfo> userLoginInfo)
        {
            var info = await _userRepository.GetOtherLoginsAsync(userLoginInfo);
            return info;
        }

        public async Task<PersonalDataViewModel> GetPersonalDataAsync(ClaimsPrincipal user)
        {
            var CurrUser = await _userRepository.GetPersonalDataAsync(user);
            if (CurrUser == null)
                throw new Exception("User in null in GetPersonalDatasync method in UserService");
            return CurrUser;
        }

        public async Task RefreshSignInAsync(string userId)
        {
            await _userRepository.RefreshSignInAsync(userId);
        }

        public async Task<IdentityResult> RemoveLoginAsync(ExternalLoginsViewModel model)
        {
            IdentityResult result = await _userRepository.RemoveLoginAsync(model);
            return result;
        } 

        public async Task<bool> UserHasPasswordAsync(ClaimsPrincipal user)
        {
            var result = await _userRepository.UserHasPasswordAsync(user);
            return result;
        }

    }
}
