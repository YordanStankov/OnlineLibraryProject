using Microsoft.AspNetCore.Mvc;
using FInalProject.Models;
using FInalProject.ViewModels;
using FInalProject.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace FInalProject.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public BooksController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
      
        //fetches all books and provides the view
        public async Task<IActionResult> AllBooks()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return RedirectToAction("LoginPlease", "UserErrors");
            }
            else if(user != null && await _userManager.GetRolesAsync(user) == null)
            {
                await _userManager.AddToRoleAsync(user, "User");
            }
           
            var books =  await _context.Books
                .Include(a => a.Author)
                .Include(bg => bg.BookGenres)
                .ThenInclude(g => g.Genre)
                .Select(n => new BookListViewModel()
                    {
                        Id = n.Id,
                        Name = n.Name,
                        Pages = n.Pages,
                        AuthorName = n.Author.Name,
                        CoverImage = n.CoverImage,
                        Genres = n.BookGenres.Select(bg => bg.Genre.Name).ToList(),
                }).ToListAsync();
            return View(books);
        }

        //fetches the book creatiion view
        public async Task<IActionResult> BookCreation()
        {
            var genres = await _context.Genres.ToListAsync(); // Fetch genres from the database

            var model = new BookCreationViewModel
            {
                GenreOptions = genres // Populate available genres
            };

            return View(model);
        }

        public async Task<IActionResult> CreateABook(BookCreationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var genres = await _context.Genres.ToListAsync();
                return View("Books", genres);
            }
            var existingAuthor = await _context.Authors.FirstOrDefaultAsync(a => a.Name == model.AuthorName);

            var correctAuthor = existingAuthor ?? new Author { Name = model.AuthorName };
            var newBook = new Book
            {
                Name = model.Name,
                ReadingTime = model.ReadingTime,
                Pages = model.Pages,
                Author = correctAuthor, 
                DateTaken = DateTime.Now,
                UntillReturn = DateTimeOffset.Now.AddDays(14), 
                CoverImage = model.CoverImage,
                Description = model.Description
            };

            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();

            foreach (var genreId in model.SelectedGenreIds)
            {
                var bookGenre = new BookGenre
                {
                    BookId = newBook.Id,
                    GenreId = genreId
                };
                _context.BookGenres.Add(bookGenre);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("BookFocus", "Books", new { Id = newBook.Id });
        }


        //fetches the book focus view and gives it data
        public async Task<IActionResult> BookFocus(int id)
        {
            var currBook = await _context.Books
                .Include(b => b.Favourites)
                .Include(b => b.Author)
                .Include(b => b.Comments)
                .ThenInclude(c => c.User)
                .Include(b => b.BookGenres)
                .ThenInclude(b => b.Genre)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (currBook == null)
            {
                return NotFound();
            }

            var bigBook = new BookFocusViewModel
            {
                BookCover = currBook.CoverImage,
                BookId = currBook.Id,
                BookName = currBook.Name,
                BookPages = currBook.Pages,
                BookAuthorName = currBook.Author.Name,
                BookReadingTime = currBook.ReadingTime,
                Description = currBook.Description,
                DateTaken = currBook.DateTaken,
                UntillReturn = currBook.UntillReturn,
                genres = currBook.BookGenres.Select(bg => bg.Genre).ToList(),
                comments =  currBook.Comments.Select(c => new CommentViewModel
                {
                    UserName = c.User.UserName ?? "Unknown User",
                    Description = c.CommentContent ?? string.Empty
                }).ToList()
            };
            return View(bigBook);
        }
       

    }
}