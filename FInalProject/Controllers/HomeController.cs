using FInalProject.Models;
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

        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View(User);
        } 

        public async Task<IActionResult> Privacy()
        {
            var user = await _userManager.GetUserAsync(User);

            await _userManager.AddToRoleAsync(user, "User");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
