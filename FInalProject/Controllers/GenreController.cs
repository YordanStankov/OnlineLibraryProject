using Microsoft.AspNetCore.Mvc;
using FInalProject.Data.Models;
using FInalProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FInalProject.Services;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using FInalProject.ViewModels.Book.BookFiltering;
using FInalProject.ViewModels.Genre.GenreOprations;

namespace FInalProject.Controllers
{
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;
        private readonly IBookOprationsService _bookOperationsService;
        public GenreController(IGenreService genreService, IBookOprationsService bookOprationsService)
        {
            _genreService = genreService;
            _bookOperationsService = bookOprationsService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public async Task<IActionResult> SpecificGenreList(int genreId, FilteringViewModel filtering)
        {
            var result = await _genreService.GetAllBooksOfCertainGenre(genreId);
            result.BooksMatchingGenre = await _bookOperationsService.ApplyFiltering(result.BooksMatchingGenre, filtering);
                return View(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGenre(int doomedGenreId)
        {
            bool succes = await _genreService.DeleteGenreAsync(doomedGenreId); 
            if(succes == true)
            {
                return RedirectToAction("GenreList");
            }
            throw new ArgumentException("couldn't delete genre");
        }

        [HttpGet]
        public async Task<IActionResult> EditGenre(int genreEditId)
        {
            var model = await _genreService.ProvideGenreForPartialAsync(genreEditId);
            return PartialView("_EditGenre", model); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveChangesToGenre(GenreEditViewModel model)
        {
            var succes = await _genreService.SaveChangesToGenreAsync(model);
            return RedirectToAction("GenreList"); 
        }
    }
}
