using FInalProject.Domain.Models;
using FInalProject.Application.Interfaces;
using FInalProject.Application.ViewModels.Admin.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FInalProject.Application.ViewModels.User.UserOperations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace FInalProject.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserStore<User> _userStore;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;    

        public UserRepository (ApplicationDbContext context, 
            UserManager<User> userManager, 
            SignInManager<User> signInManager, 
            IUserStore<User> userStore)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _userStore = userStore;
        }

        public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordOperationViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            return await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }

        public async Task<ChangePasswordViewModel> GetChangePasswordAsync(ClaimsPrincipal user)
        {
            ChangePasswordViewModel result = new ChangePasswordViewModel();
            var CurrUser = await _userManager.GetUserAsync(user);
            result.Id = CurrUser.Id; 
            return result;
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("Unable to find user by id in GetLoginsAsync in UserRepository");
            var list = await _userManager.GetLoginsAsync(user);
            return list;
        }

        public async Task<IList<AuthenticationScheme>> GetOtherLoginsAsync(IList<UserLoginInfo> userLoginInfo)
        {
            var OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => userLoginInfo.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            return OtherLogins;
        }

        public async Task<string?> GetPasswordHashAsync(string userId, CancellationToken cancellationToken)
        {
            var user = _userManager.FindByIdAsync(userId);
            if (_userStore is IUserPasswordStore<User> passwordStore)
            {
                return await passwordStore.GetPasswordHashAsync(user, cancellationToken);
            }

            return null;
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

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task RefreshSignInAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User is null at RefreshSignInAsync in UserRepository");
            await _signInManager.RefreshSignInAsync(user);
        }

        public async Task<List<UserListViewModel>> RenderUsersInViewModelAsync()
        {
            return await _context.Users.Select(u => new UserListViewModel
            {
                UserId = u.Id,
                UserName = u.UserName,
                Strikes = u.Strikes ?? 0,
                CantBorrow = u.CantBorrow
            }).ToListAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
             _context.Users.Update(user);
            await _context.SaveChangesAsync();  
        }

        public async Task<bool> UserHasPasswordAsync(ClaimsPrincipal user)
        {
            var UserModel = await _userManager.GetUserAsync(user);
            if (UserModel == null) throw new Exception("User is null in UserHasPasswordAsync in UserRepostiory");
            var hasPassword = await _userManager.HasPasswordAsync(UserModel);
            if (hasPassword == false) return false;
            else 
                return true;
        }
    }
}
