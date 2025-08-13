using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FInalProject.Application.Services;
using FInalProject.Application.ViewModels.Book;

namespace FInalProject.Web.Areas.Identity.Pages.Account.Manage
{
    public class LikedBooksModel : PageModel
    {
        private readonly IFavouriteService _favouriteService;
        public LikedBooksModel(IFavouriteService favouriteService)
        {
            _favouriteService = favouriteService;
        }

        public List<LikedBookListViewModel>? FavouritesOfUser { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
                FavouritesOfUser = await _favouriteService.ReturnLikedBookListAsync(User);
                return Page();
        }
    }
}
