using FInalProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace FInalProject.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IAuthorsService _authorsService;
        public AuthorsController(IAuthorsService authorsService)
        {
            _authorsService = authorsService;
        }
        public async Task<IActionResult> AllAuthors()
        {
            var authors = await _authorsService.RenderAuthorListAsync();
            return View(authors);
        }
        public async Task<IActionResult> AuthorProfile(int authorId)
        {
            var author = await _authorsService.RenderAuthorProfileAsync(authorId);
            return View(author);
        }
    }
}
