using FInalProject.Data.Models;
using FInalProject.Services;
using FInalProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace FInalProject.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IAuthorsService _authorsService;
        private readonly UserManager<User> _userManager;
        public AuthorsController(IAuthorsService authorsService, UserManager<User> userManager)
        {
            _authorsService = authorsService;
            _userManager = userManager;
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
        public async Task<IActionResult> AddAuthorPortrait(AddAuthorPortraitViewModel model)
        {
            var response = await _authorsService.AddPortraitToAuthorAsync(model);
            return RedirectToAction("AuthorProfile", new { authorId = model.Id });
        }
        [HttpGet]
        public async Task<IActionResult> SearchAuthor(string searchedString)
        {
            var authors = await _authorsService.RenderSearchResultsAsync(searchedString);
            TempData["SearchedAuthors"] = JsonConvert.SerializeObject(authors);
            return RedirectToAction("AllAuthors");
        }
    }
}
