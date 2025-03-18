using FInalProject.Data;
using FInalProject.Models;
using FInalProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FInalProject.Services
{
    public interface IBooksService
    {
        Task<List<BookListViewModel>> ReturnSearchResultsAync(string searchedString);
        Task<List<BookListViewModel>> GetAllBooksAsync(ClaimsPrincipal user);
        Task<BookFocusViewModel> GetBookFocusAsync(int id);
        Task<BookCreationViewModel> GetBookCreationViewModelAsync();
        Task<int> CreateBookAsync(BookCreationViewModel model);
    }
    public class BooksService : IBooksService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager; 

        public BooksService(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<BookListViewModel>> GetAllBooksAsync(ClaimsPrincipal user)
        {
            var currUser = await _userManager.GetUserAsync(user);
            if (currUser == null)
            {
                return null;
            }
            else if (await _userManager.GetRolesAsync(currUser) == null)
            {
                await _userManager.AddToRoleAsync(currUser, "User");
            }

            return await _context.Books
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
        }


        public async Task<int> CreateBookAsync(BookCreationViewModel model)
        {
            if(model == null || model.SelectedGenreIds == null)
            {
                throw new ArgumentException("Invalid book data or no selected genres");
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
            return newBook.Id;
        }

        
        public async Task<BookCreationViewModel> GetBookCreationViewModelAsync()
        {
            var genres = await _context.Genres.ToListAsync();
            return new BookCreationViewModel
            {
                GenreOptions = genres
            };
        }

        public async Task<BookFocusViewModel> GetBookFocusAsync(int id)
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
                return null;
            }

            return new BookFocusViewModel
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
                comments = currBook.Comments.Select(c => new CommentViewModel
                {
                    UserName = c.User.UserName ?? "Unknown User",
                    Description = c.CommentContent ?? string.Empty
                }).ToList()
            };
        }
        public async Task<List<BookListViewModel>> ReturnSearchResultsAync(string searchedString)
        {
            string loweredSearch = searchedString.ToLower();
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.BookGenres)
                .ThenInclude(b => b.Genre)
                .Where(b => b.Author.Name.ToLower().Contains(loweredSearch)
                || b.Name.ToLower().Contains(loweredSearch))
                .Select(b => new BookListViewModel()
                {
                    Id = b.Id,
                    Name = b.Name,
                    Pages = b.Pages,
                    AuthorName = b.Author.Name,
                    CoverImage = b.CoverImage,
                    Genres = b.BookGenres.Select(bg => bg.Genre.Name).ToList(),
                })
                .ToListAsync();
        }
    }
}
