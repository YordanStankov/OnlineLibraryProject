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
            string? BookJson = TempData["Books"] as string;
            if(BookJson == null)
            {
                if (modifier == 0)
                {
                    var bookies = await _booksService.GetAllBooksAsync();
                    if (bookies == null)
                    {
                        return RedirectToAction("LoginPlease", "UserErrors");
                    }
                    return View(bookies);
                }
                else 
                {
                    var specificBooks = await _booksService.GetAllBooksFromSpecificCategoryAsync(modifier);
                    if (specificBooks == null)
                    {
                        return RedirectToAction("NoneFromCategory", "UserErrors");
                    }
                    return View(specificBooks);
                }
            }
            else
            {
               
                List<BookListViewModel>? books = new List<BookListViewModel>();
                    books = JsonConvert.DeserializeObject<List<BookListViewModel>>(BookJson);
                return View(books);
            }
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
            int bookId = await _booksService.CreateBookAsync(model);
            return RedirectToAction("BookFocus", "Books", new { Id = bookId });
        }

        [HttpGet]
        public async Task<IActionResult> BookFocus(int id)
        {
            bool response = await _booksService.UserRoleCheckAsync(User);
            if(response == true)
                {
                    ViewBag.IsHe = true; 
                }
            else
                {
                    ViewBag.IsHe = false;  
                }

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
        [HttpGet]
        public async Task<IActionResult> BooksLeaderboard()
        {
            var bookies = await _booksService.ReturnLeaderboardResultsAsync();
            return View(bookies);
        }
    }
}