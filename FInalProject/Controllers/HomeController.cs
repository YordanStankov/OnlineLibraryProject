using FInalProject.Data.Models;
using FInalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FInalProject.Data;
using Microsoft.AspNetCore.Identity;

namespace FInalProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context; 

        }
        public async Task<IActionResult> Index()
        {
            var currUser = await _userManager.GetUserAsync(User);
            if (currUser == null)
            {
                return View();
            }
            var roles = await _userManager.GetRolesAsync(currUser);
                if (!roles.Contains("User") && !roles.Contains("Admin"))
                {
                    await _userManager.AddToRoleAsync(currUser, "User");
                }
            return View();
        } 

        public async Task<IActionResult> Privacy()
        {
          
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
