using Microsoft.AspNetCore.Mvc;
using FInalProject.Application.Services;
using FInalProject.Application.ViewModels.Book.BookFiltering;
using FInalProject.Application.ViewModels.Genre.GenreOprations;

namespace FInalProject.Web.Controllers
{
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;
        private readonly IBookFilteringService _bookFilteringService;
        public GenreController(IGenreService genreService, IBookFilteringService bookFilteringService)
        {
            _genreService = genreService;
            _bookFilteringService = bookFilteringService;
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
            result.BooksMatchingGenre = await _bookFilteringService.ApplyFiltering(result.BooksMatchingGenre, filtering);
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
