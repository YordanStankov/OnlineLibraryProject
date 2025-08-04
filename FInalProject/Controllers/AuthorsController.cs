using FInalProject.Application.Services;
using FInalProject.Application.ViewModels.Author;
using FInalProject.Application.ViewModels.Author.AuthorOperations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FInalProject.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IAuthorsService _authorsService;
        
        public AuthorsController(IAuthorsService authorsService)
        {
            _authorsService = authorsService;
        }

        [HttpGet]
        public async Task<IActionResult> AllAuthors()
        {
            string? authorsJson = TempData["SearchedAuthors"] as string;
            if (authorsJson == null)
            {
                var authors = await _authorsService.RenderAuthorListAsync();
                return View(authors);
            }
            List<AuthorListViewModel>? searchedAuthors = new List<AuthorListViewModel>();
            searchedAuthors = JsonConvert.DeserializeObject<List<AuthorListViewModel>>(authorsJson);
            return View(searchedAuthors);
        }

        [HttpGet]
        public async Task<IActionResult> AuthorProfile(int authorId)
        {
            var author = await _authorsService.RenderAuthorProfileAsync(authorId, User);
            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FavouriteAuthor(int authorId)
        {
            bool response = await _authorsService.FavouriteAuthorAsync(authorId, User);
            if(response == false)
            {
                throw new ArgumentNullException("user is null");
            }
            return Json(new { success = response });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAuthorPortrait(AddAuthorPortraitViewModel model)
        {
            var response = await _authorsService.AddPortraitToAuthorAsync(model);
            return RedirectToAction("AuthorProfile", new { authorId = model.Id });
        }

        [HttpGet]
        public async Task<IActionResult> AuthorSearchResults(string searchedString)
        {
            var model = await _authorsService.RenderSearchResultsAsync(searchedString);
            return View(model);
        }
    }
}
