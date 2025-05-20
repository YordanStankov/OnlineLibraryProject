using Microsoft.AspNetCore.Mvc;
using FInalProject.Data.Models;
using FInalProject.ViewModels;
using FInalProject.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using FInalProject.Services;
using Newtonsoft.Json;

using System.Threading.Tasks;

namespace FInalProject.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBooksService _booksService;
        private readonly IBookOprationsService _bookOperationsService;
        public BooksController(IBooksService bookService, IBookOprationsService bookOperationsService)
        {
            _booksService = bookService;
            _bookOperationsService = bookOperationsService;
        }

        [HttpGet]
        public async Task<IActionResult> AllBooks(FilteringViewModel filtering)
        {
            var books = await _booksService.GetAllBooksAsync();
            var bookies = await _bookOperationsService.ApplyFiltering(books, filtering);
                return View(bookies);
        }

        [HttpGet]
        public async Task<IActionResult> BooksFromSpecificCategory(int modifier, FilteringViewModel filtering)
        {
            var results = await _booksService.GetAllBooksFromSpecificCategoryAsync(modifier);
            if(results.BooksFromCategory != null)
            {
                results.BooksFromCategory = await _bookOperationsService.ApplyFiltering(results.BooksFromCategory, filtering);
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
            int bookId = await _booksService.CreateBookAsync(model);
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
            var results = await _bookOperationsService.ReturnSearchResultsAync(searchedString);
            if (results?.BooksMatchingQuery != null)
            {
                results.BooksMatchingQuery = await _bookOperationsService.ApplyFiltering(results.BooksMatchingQuery, filtering);
            }
            return View(results);
        }
    }
}