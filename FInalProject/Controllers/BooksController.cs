using Microsoft.AspNetCore.Mvc;
using FInalProject.Models;
using FInalProject.ViewModels;
using FInalProject.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace FInalProject.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public BooksController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AllBooks()
        {
            return View();
        }
        public IActionResult BookCreation()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateABook(BookCreationViewModel Book)
        {
            Book BookFloat = new Book()
            {
                Name = Book.Name,
                Author = Book.Author,
                Description = Book.Description,
                ReadingTime = Book.ReadingTime,
                CoverImage = Book.CoverImage,
                Pages = Book.Pages
            };
            _context.Add(BookFloat);
            _context.SaveChanges();
            return RedirectToAction("Index"); 
        }
    }
}
