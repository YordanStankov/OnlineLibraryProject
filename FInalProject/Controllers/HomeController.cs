using FInalProject.Data.Models;
using FInalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FInalProject.Data;
using Microsoft.AspNetCore.Identity;
using FInalProject.Services;

namespace FInalProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;
        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            var assigned = await _homeService.AssignRoleAsync(User);
            return View();
        } 

        public async Task<IActionResult> Privacy()
        {
          
            return View();
        }
        public async Task<IActionResult> BorrowingPolicy()
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
