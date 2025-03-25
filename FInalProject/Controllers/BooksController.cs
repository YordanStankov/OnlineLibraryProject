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

namespace FInalProject.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBooksService _booksService;

        public BooksController(IBooksService bookService)
        {
            _booksService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> AllBooks(int modifier)
        {
            if(modifier == 0)
            {
                var books = await _booksService.GetAllBooksAsync(User);
                if (books == null)
                {
                    return RedirectToAction("LoginPlease", "UserErrors");
                }
                return View(books);
            }

            var specificBooks = await _booksService.GetAllBooksFromSpecificCategoryAsync(modifier);
            if(specificBooks == null)
                {
                    return RedirectToAction("NoneFromCategory", "UserErrors");
                }
            return View(specificBooks);
            
        }

        [HttpGet]
        public async Task<IActionResult> BookCreation()
        {
            var model = await _booksService.GetBookCreationViewModelAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateABook(BookCreationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Books", await _booksService.GetBookCreationViewModelAsync());
            }
            int bookId = await _booksService.CreateBookAsync(model);
            return RedirectToAction("BookFocus", "Books", new { Id = bookId });
        }

        [HttpGet]
        public async Task<IActionResult> SearchedBookList()
        {
            var booksJson = TempData["Books"] as string;
            List<BookListViewModel> books = new List<BookListViewModel>();
            if(booksJson != null)
            {
                books = JsonConvert.DeserializeObject<List<BookListViewModel>>(booksJson);
            }
            return View(books); 
        }

        [HttpGet]
        public async Task<IActionResult> BookFocus(int id)
        {
            var focusedBook = await _booksService.GetBookFocusAsync(id);
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
    }
}