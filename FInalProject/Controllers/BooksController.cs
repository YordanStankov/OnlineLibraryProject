using Microsoft.AspNetCore.Mvc;
using FInalProject.Models;
using FInalProject.ViewModels;
using FInalProject.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;


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

        public IActionResult AddGenre()
        {
            return PartialView();
        }


        [HttpPost]
        public IActionResult AddingAGenre(AddGenreViewModel viewGenre)
        {
           
            Genre GenreFloat = new Genre()
            {
                Name = viewGenre.Name
            };
            _context.Add(GenreFloat);
            _context.SaveChanges(); 
            return RedirectToAction();
        }
       
        //fetches all books and provdes the view
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
        public IActionResult BookCreation()
        {
            return View();
        }

        //actually creates the book
        [HttpPost]
        public IActionResult CreateABook(BookCreationViewModel Book)
        {
            Book BookFloat = new Book()
            {
                Name = Book.Name,
                Author = Book.Author,
                Description = Book.Description,
                ReadingTime = Book.ReadingTime,
                CoverImage = Book.CoverImage,
                Pages = Book.Pages
            };
            _context.Add(BookFloat);
            _context.SaveChanges();
            return RedirectToAction("AllBooks"); 
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
                DateTaken = DateTime.Today,
                UntillReturn = new DateTimeOffset(DateTime.Today),
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