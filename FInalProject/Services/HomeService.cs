using System.Diagnostics;
using System.Security.Claims;
using FInalProject.Controllers;
using FInalProject.Data;
using FInalProject.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FInalProject.Services
{
    
    public interface IHomeService
    {
        Task<bool> AssignRoleAsync(ClaimsPrincipal User);
    }
    public class HomeService : IHomeService
    {
        private readonly ILogger<HomeService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public HomeService( UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, ILogger<HomeService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
        }
        public async Task<bool> AssignRoleAsync(ClaimsPrincipal User)
        {
            var currUser = await _userManager.GetUserAsync(User);
            _logger.LogInformation($"ASSIGNING ROLE TO USER:{currUser}");

            if (currUser != null)
            {
                var roles = await _userManager.GetRolesAsync(currUser);
                if (!roles.Contains("User") && !roles.Contains("Admin") && !roles.Contains("Librarian"))
                {
                    await _userManager.AddToRoleAsync(currUser, "User");
                    _logger.LogInformation($"USER: {currUser} IS NOW A USER");
                    return true;
                }
                _logger.LogInformation("USER ALREADY IN ROLE");
                return true;
                
            }
            _logger.LogInformation("USER IS NULL REGISTER");
            return false; 
        }
    }
}
