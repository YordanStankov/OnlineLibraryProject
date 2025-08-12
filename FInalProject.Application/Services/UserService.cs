
using FInalProject.Application.ViewModels.User.UserOperations;
using FInalProject.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FInalProject.Application.Services
{
    public interface IUserService
    {
        Task<PersonalDataViewModel> GetPersonalDataAsync(ClaimsPrincipal user);
    }
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        public UserService(UserManager<User> userManager) 
        { 
            _userManager = userManager; 
        }

        public async Task<PersonalDataViewModel> GetPersonalDataAsync(ClaimsPrincipal user)
        {
             var CurrUser = await _userManager.GetUserAsync(user);
            if (CurrUser == null)
                return null;
            PersonalDataViewModel viewModel = new PersonalDataViewModel
            {
                Id = CurrUser.Id,
                Email = CurrUser.Email,
                UserName = CurrUser.UserName
            };
            return viewModel;
        }
    }
}
