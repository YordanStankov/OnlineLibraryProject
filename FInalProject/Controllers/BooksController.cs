using Microsoft.AspNetCore.Mvc;
using FInalProject.Models;
using FInalProject.ViewModels;


namespace FInalProject.Controllers
{
    public class BooksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AllBooks()
        {
            return View();
        }
    }
}
