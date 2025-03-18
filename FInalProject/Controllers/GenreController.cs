using Microsoft.AspNetCore.Mvc;
using FInalProject.ViewModels;
using FInalProject.Models;
using FInalProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FInalProject.Services;

namespace FInalProject.Controllers
{
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpPost]
        public async Task<IActionResult> AddGenre(string Name)
        {
            int response = await _genreService.AddGenreAsync(Name);
            if(response == 2)
            {
                throw new ArgumentException("couldn't add genre");
            }
            return RedirectToAction("GenreList", "Genre");
        }

        [HttpGet]
        public async Task<IActionResult> GenreList()
        {
            var genres = await _genreService.GetGenreListAsync();
            return View(genres);
        }

        [HttpGet]
        public async Task<IActionResult> SpecificGenreList(int genreId)
        {
            var result = await _genreService.GetAllBooksOfCertainGenre(genreId);
            if(result == null)
            {
                throw new ArgumentException("No books associated with the genre");
            }
                return View(result);
        }
    }
}
