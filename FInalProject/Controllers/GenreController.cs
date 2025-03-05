using Microsoft.AspNetCore.Mvc;
using FInalProject.ViewModels;
using FInalProject.Models;
using FInalProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FInalProject.Controllers
{
    public class GenreController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public GenreController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddGenre(string Name)
        {
            Genre genreFloat = new Genre()
            {
                Name = Name,
            };
            _context.Add(genreFloat);
            _context.SaveChanges();
            return RedirectToAction("GenreList", "Genre");
        }

        public async Task<IActionResult> GenreList()
        {
            var Genres = await _context.Genres.ToListAsync(); 
            return View(Genres);
        }
    }
}
