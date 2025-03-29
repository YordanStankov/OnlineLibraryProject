using System.Diagnostics;
using System.Security.Claims;
using FInalProject.Controllers;
using FInalProject.Data;
using FInalProject.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace FInalProject.Services
{
    
    public interface IHomeService
    {
        Task<bool> AssignRoleAsync(ClaimsPrincipal User);
    }
    public class HomeService : IHomeService
    {
        
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public HomeService( UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;

        }
        public async Task<bool> AssignRoleAsync(ClaimsPrincipal User)
        {
            var currUser = await _userManager.GetUserAsync(User);
            if (currUser == null)
            {
                return false;
            }
            var roles = await _userManager.GetRolesAsync(currUser);
            if (!roles.Contains("User") && !roles.Contains("Admin"))
            {
                await _userManager.AddToRoleAsync(currUser, "User");
                return true;
            }
            return false; 
        }
    }
}
