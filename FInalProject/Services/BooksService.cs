using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace FInalProject.Services
{
    public interface IBooksService
    {
        Task<List<BookListViewModel>> GetAllBooksFromSpecificCategoryAsync(int modifier);
        Task<BookCreationViewModel> getBookInfoAsync(int editId);
        Task<List<BookListViewModel>> GetAllBooksAsync();
        Task<BookFocusViewModel> GetBookFocusAsync(int id);
        Task<BookCreationViewModel> GetBookCreationViewModelAsync();
        Task<bool> UserRoleCheckAsync(ClaimsPrincipal user);
        Task<int> CreateBookAsync(BookCreationViewModel model);
        Task<List<BooksLeaderboardViewModel>> ReturnLeaderboardResultsAsync();
    }
    public class BooksService : IBooksService
    {
        public readonly ILogger<BooksService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager; 

        public BooksService(ApplicationDbContext context, UserManager<User> userManager, ILogger<BooksService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<List<BookListViewModel>> GetAllBooksAsync()
        {
            _logger.LogInformation("GETTING ALL BOOKS");
            return await _context.Books
                .AsNoTracking()
                .Include(a => a.Author)
                .Include(bg => bg.BookGenres)
                .ThenInclude(g => g.Genre)
                .Where(b => b.AmountInStock > 0)
                .Select(n => new BookListViewModel()
                {
                    Id = n.Id,
                    Name = n.Name,
                    Pages = n.Pages,
                    Category = n.Category,
                    AuthorName = n.Author.Name,
                    CoverImage = n.CoverImage,
                    Genres = n.BookGenres.Select(bg => bg.Genre.Name).ToList(),
                }).ToListAsync();
        }
        public async Task<int> CreateBookAsync(BookCreationViewModel model)
        {
            _logger.LogDebug("LOG DEBUG CREATING BOOK ASYNC");
            var existingAuthor = await _context.Authors.FirstOrDefaultAsync(a => a.Name == model.AuthorName);
            var correctAuthor = existingAuthor ?? new Author { Name = model.AuthorName };
            var newBook = new Book
            {
                Name = model.Name,
                ReadingTime = model.ReadingTime,
                Pages = model.Pages,
                Author = correctAuthor,
                AmountInStock = model.AmountInStock,
                Category = model.Category,
                CategoryString = model.Category.ToString(),
                CoverImage = model.CoverImage,
                Description = model.Description
            };

            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();
            if (model.SelectedGenreIds != null)
            {
                foreach (var genreId in model.SelectedGenreIds)
                {
                    var bookGenre = new BookGenre
                    {
                        BookId = newBook.Id,
                        GenreId = genreId
                    };
                    _context.BookGenres.Add(bookGenre);
                }
            }
            newBook.Author.Books.Add(newBook);
            await _context.SaveChangesAsync();
            return newBook.Id;
        }

        
        public async Task<BookCreationViewModel> GetBookCreationViewModelAsync()
        {
            _logger.LogInformation("FILLING THE VIEW WITH GENRE OPTIONS");
            var genres = await _context.Genres.AsNoTracking().ToListAsync();
            return new BookCreationViewModel
            {
                GenreOptions = genres
            };
        }

        public async Task<BookFocusViewModel> GetBookFocusAsync(int id)
        {
            _logger.LogInformation("GETTING BOOK FOCUS FILLING THE VIEW");
            var currBook = await _context.Books
               .AsNoTracking()
              .Include(b => b.Favourites)
              .Include(b => b.Author)
              .Include(b => b.Comments)
              .ThenInclude(c => c.User)
              .Include(b => b.BookGenres)
              .ThenInclude(b => b.Genre)
              .FirstOrDefaultAsync(b => b.Id == id);

            if (currBook == null)
            {
                return null;
            }

            return new BookFocusViewModel
            {
                BookCover = currBook.CoverImage,
                BookId = currBook.Id,
                BookName = currBook.Name,
                BookPages = currBook.Pages,
                Category = currBook.Category,
                BookAuthorName = currBook.Author.Name,
                AmountInStock = currBook.AmountInStock,
                BookReadingTime = currBook.ReadingTime,
                Description = currBook.Description,
                
                genres = currBook.BookGenres.Select(bg => bg.Genre).ToList(),
                comments = currBook.Comments.Select(c => new CommentViewModel
                {
                    UserName = c.User.UserName ?? "Unknown User",
                    Description = c.CommentContent ?? string.Empty
                }).ToList(),
                Favourites = currBook.Favourites
            };
        }

        public async Task<BookCreationViewModel> getBookInfoAsync(int editId)
        {
            var book = await _context.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.BookGenres)
                .ThenInclude(b => b.Genre)
                .FirstOrDefaultAsync(b => b.Id == editId);
            
            var genres = await _context.Genres.ToListAsync();

            return new BookCreationViewModel
            {
                Id = book.Id,
                Name = book.Name, 
                Category = book.Category,
                AuthorName = book.Author.Name,
                AmountInStock = book.AmountInStock, 
                CoverImage = book.CoverImage,
                Description = book.Description,
                Pages = book.Pages, 
                ReadingTime = book.ReadingTime,
                GenreOptions = genres,
            };
        }

        public async Task<List<BookListViewModel>> GetAllBooksFromSpecificCategoryAsync(int modifier)
        {
            _logger.LogInformation("GETTING ALL BOOKS FROM A CERTAIN GENRE FILLING THE VIEW");
            var specificBooks = await _context.Books
                .AsNoTracking()
                .Include(a => a.Author)
                .Include(bg => bg.BookGenres)
                .ThenInclude(g => g.Genre)
                .Where(b => (int)b.Category == modifier && b.AmountInStock > 0)
                .Select(n => new BookListViewModel()
                {
                    Id = n.Id,
                    Name = n.Name,
                    Pages = n.Pages,
                    AuthorName = n.Author.Name,
                    Category = n.Category,
                    CoverImage = n.CoverImage,
                    Genres = n.BookGenres.Select(bg => bg.Genre.Name).ToList(),
                }).ToListAsync();
            if (specificBooks == null)
            {
                return null;
            }
            return specificBooks; 
        }
        public async Task<bool> UserRoleCheckAsync(ClaimsPrincipal user)
        {
            var currUser = await _userManager.GetUserAsync(user);
            if (await _userManager.IsInRoleAsync(currUser, "User"))
            {
                return true;
            }
            return false;
        }

        public async Task<List<BooksLeaderboardViewModel>> ReturnLeaderboardResultsAsync()
        {
            return await _context.Books
                .OrderByDescending(b => b.Favourites.Sum(f => f.Amount))
                .Select( b => new BooksLeaderboardViewModel
                {
                    BookId = b.Id,
                    AuthorName = b.Author.Name,
                    BookName = b.Name,
                    CategoryString = b.CategoryString,
                    PositiveReviews = b.Favourites.Sum(f => f.Amount)
                })
                .ToListAsync();
        }
    }
}
