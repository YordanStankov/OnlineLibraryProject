using FInalProject.Data.Models;
using FInalProject.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            var authors = await _authorsService.RenderAuthorListAsync();
            return View(authors);
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
    }
}
