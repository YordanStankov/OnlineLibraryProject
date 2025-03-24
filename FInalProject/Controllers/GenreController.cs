using Microsoft.AspNetCore.Mvc;
using FInalProject.ViewModels;
using FInalProject.Data.Models;
using FInalProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FInalProject.Services;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
            bool response = await _genreService.AddGenreAsync(Name);
            if (response == false)
            {
                throw new ArgumentException("already exists dumbass");
            }
            return RedirectToAction("GenreList");
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
        [HttpPost]
        public async Task<IActionResult> DeleteGenre(int doomedGenreId)
        {
            bool succes = await _genreService.DeleteGenreAsync(doomedGenreId); 
            if(succes == true)
            {
                return RedirectToAction("GenreList");
            }
            throw new ArgumentException("couldn't delete genre");
        }
        
        public async Task<IActionResult> EditGenre()
        {
            return RedirectToAction("GenreList");
        }
    }
}
