using FInalProject.Application.Services;
using FInalProject.Application.ViewModels.Book.BookOperations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FInalProject.Application.ViewModels.Book;
namespace FInalProject.Web.Areas.Identity.Pages.Account.Manage
{
    public class BorrowedBooksModel : PageModel
    {
        private readonly IBookHandlingService _bookHandlingService;
        private readonly IBooksService _booksService;


        public BorrowedBooksModel (IBookHandlingService bookHandlingService, IBooksService booksService)
        {
            _bookHandlingService = bookHandlingService;
            _booksService = booksService;
        }

        public List<BorrowedBookListViewModel> BorrowedBooksFromUser { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            BorrowedBooksFromUser = await _booksService.ReturnBorrowedBookListAsync(User);
                return Page();
        }

        [BindProperty]
        public ReturnBookViewModel model { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            bool response = await _bookHandlingService.ReturnBookAsync(model, User);
            return RedirectToPage();
        }
    }
}
