using Microsoft.AspNetCore.Mvc;
using FInalProject.Application.Services;
using FInalProject.Application.ViewModels.Book.BookFiltering;
using FInalProject.Application.ViewModels.Book;

namespace FInalProject.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBooksService _booksService;
        private readonly IBookCRUDService _bookCRUDService;
        private readonly IBookFilteringService _bookFilteringService;
        public BooksController(IBooksService bookService, IBookCRUDService bookCRUDService, IBookFilteringService bookFilteringService)
        {
            _booksService = bookService;
            _bookCRUDService = bookCRUDService;
            _bookFilteringService = bookFilteringService;
        }

        [HttpGet]
        public async Task<IActionResult> AllBooks(FilteringViewModel filtering)
        {
            var books = await _booksService.GetAllBooksAsync();
            var bookies = await _bookFilteringService.ApplyFiltering(books, filtering);
                return View(bookies);
        }

        [HttpGet]
        public async Task<IActionResult> BooksFromSpecificCategory(int modifier, FilteringViewModel filtering)
        {
            var results = await _booksService.GetAllBooksFromSpecificCategoryAsync(modifier);
            if(results.BooksFromCategory != null)
            {
                results.BooksFromCategory = await _bookFilteringService.ApplyFiltering(results.BooksFromCategory, filtering);
            }
            
            return View(results);
        }

        [HttpGet]
        public async Task<IActionResult> BookCreation()
        {
            var model = await _booksService.GetBookCreationViewModelAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateABook(BookCreationViewModel model)
        {
            int bookId = await _bookCRUDService.CreateBookAsync(model);
            return RedirectToAction("BookFocus", "Books", new { Id = bookId });
        }

        [HttpGet]
        public async Task<IActionResult> BookFocus(int id)
        {
            bool response = await _booksService.UserRoleCheckAsync(User);
            if (response == true)
            {
                ViewBag.IsHe = true;
            }
            else
            {
                ViewBag.IsHe = false;
            }
            bool answer = await _booksService.CheckIfUserCantBorrowAsync(User);
            if (!answer) ViewBag.CantBorrow = false;
            if (answer) ViewBag.CantBorrow = true; 

            var focusedBook = await _booksService.GetBookFocusAsync(id, User);

            if (focusedBook == null)
            {
                throw new ArgumentException("Book not found");
            }
            return View(focusedBook);
        }

        [HttpGet]
        public async Task<IActionResult> EditBook(int editId)
        {
            var info = await _booksService.getBookInfoAsync(editId);
            return View(info);
        }

        [HttpGet]
        public async Task<IActionResult> BooksLeaderboard()
        {
            var bookies = await _booksService.ReturnLeaderboardResultsAsync();
            return View(bookies);
        }

        [HttpGet]
        public async Task<IActionResult> SearchResults(string searchedString ,FilteringViewModel filtering)
        {
            var results = await _bookFilteringService.ReturnSearchResultsAync(searchedString);
            if (results?.BooksMatchingQuery != null)
            {
                results.BooksMatchingQuery = await _bookFilteringService.ApplyFiltering(results.BooksMatchingQuery, filtering);
            }
            return View(results);
        }
    }
}