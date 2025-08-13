
using FInalProject.Application.Interfaces;
using FInalProject.Application.ViewModels.User.UserOperations;
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
        Task<IdentityResult> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository) 
        { 
            _userRepository = userRepository; 
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

        public async Task<ChangePasswordViewModel> GetChangePasswordAsync(ClaimsPrincipal user)
        {
            var CurrUser = await _userRepository.GetChangePasswordAsync(user);
            if (CurrUser == null)
                throw new Exception("User is null in GetChangePasswordASync method in UserService");
            return CurrUser;
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

        public async Task<bool> UserHasPasswordAsync(ClaimsPrincipal user)
        {
            var result = await _userRepository.UserHasPasswordAsync(user);
            return result;
        }

    }
}
