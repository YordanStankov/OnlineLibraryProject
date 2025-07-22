using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.Repositories.Interfaces;
using FInalProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace FInalProject.Services
{
    public interface IBooksService
    {
        Task<BooksFromCategoryViewModel> GetAllBooksFromSpecificCategoryAsync(int modifier);
        Task<BookCreationViewModel> getBookInfoAsync(int editId);
        Task<List<BookListViewModel>> GetAllBooksAsync();
        Task<BookFocusViewModel> GetBookFocusAsync(int id, ClaimsPrincipal User);
        Task<BookCreationViewModel> GetBookCreationViewModelAsync();
        Task<bool> UserRoleCheckAsync(ClaimsPrincipal user);
        Task<int> CreateBookAsync(BookCreationViewModel model);
        Task<List<BooksLeaderboardViewModel>> ReturnLeaderboardResultsAsync();
        Task<bool> CheckIfUserCantBorrowAsync(ClaimsPrincipal User);
    }
    public class BooksService : IBooksService
    {
        public readonly ILogger<BooksService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IBorrowedBookRepository _borrowedBookRepository;
        private readonly IBookGenreRepository _bookGenreRepository;

        public BooksService(ApplicationDbContext context, UserManager<User> userManager, ILogger<BooksService> logger, IBookRepository bookRepository, IGenreRepository genreRepository, IAuthorRepository authorRepository, IBookGenreRepository bookGenreRepository, IBorrowedBookRepository borrowedBookRepository )
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _bookRepository = bookRepository;
            _genreRepository = genreRepository;
            _authorRepository = authorRepository;
            _bookGenreRepository = bookGenreRepository;
            _borrowedBookRepository = borrowedBookRepository;
        }

        public async Task<List<BookListViewModel>> GetAllBooksAsync()
        {
            _logger.LogInformation("GETTING ALL BOOKS");
           return await _bookRepository.RenderBookListAsync();
        }
        public async Task<int> CreateBookAsync(BookCreationViewModel model)
        {
            _logger.LogDebug("LOG DEBUG CREATING BOOK ASYNC");
            var existingAuthor = await _authorRepository.GetAuthorByNameAsync(model.AuthorName);
            var correctAuthor = existingAuthor ?? new Author { Name = model.AuthorName };
            var newBook = new Book
            {
                Name = model.Name,
                ReadingTime = model.ReadingTime,
                Pages = model.Pages,
                Author = correctAuthor,
                DateWritten = model.DateWritten,
                AmountInStock = model.AmountInStock,
                Category = model.Category,
                CategoryString = model.Category.ToString(),
                CoverImage = model.CoverImage,
                Description = model.Description
            };

            await _bookRepository.AddBookAsync(newBook);
            if (model.SelectedGenreIds != null)
            {
                foreach (var genreId in model.SelectedGenreIds)
                {
                    var bookGenre = new BookGenre
                    {
                        BookId = newBook.Id,
                        GenreId = genreId
                    };
                  await _bookGenreRepository.AddNewBookGenre(bookGenre);
                }
            }
            await _authorRepository.AddToAuhtorBookListAsync(correctAuthor, newBook);
            return newBook.Id;
        }

        
        public async Task<BookCreationViewModel> GetBookCreationViewModelAsync()
        {
            _logger.LogInformation("FILLING THE VIEW WITH GENRE OPTIONS");

            return new BookCreationViewModel
            {
                GenreOptions = await _genreRepository.GetAllGenresAsync()
            };
        }

        public async Task<BookFocusViewModel> GetBookFocusAsync(int id, ClaimsPrincipal User)
        {
            BookFocusViewModel focusModel = new BookFocusViewModel();
            var curr = _userManager.GetUserId(User);

            if(curr == null)
            {
                return focusModel;
            }
            _logger.LogInformation("GETTING BOOK FOCUS FILLING THE VIEW");
            var currBook = await _bookRepository.GetSingleBookAsync(id);

            if (currBook == null)
            {
                return null;
            }
            var borrow = await _borrowedBookRepository.GetSingleBorrowedBookAsync(curr, currBook.Id);
            if(borrow == null)
            {
                focusModel.Borrowed = false;
            }
            else
            {
                focusModel.Borrowed = true;
            }
             
                {
                focusModel.BookCover = currBook.CoverImage;
                focusModel.BookId = currBook.Id;
                focusModel.BookName = currBook.Name;
                focusModel.BookPages = currBook.Pages;
               focusModel.Category = currBook.Category;
                focusModel.DateWritten = currBook.DateWritten;
                focusModel.BookAuthorName = currBook.Author.Name;
                focusModel.AmountInStock = currBook.AmountInStock;
                focusModel.BookReadingTime = currBook.ReadingTime;
                focusModel.Description = currBook.Description;

                focusModel.genres = currBook.BookGenres.Select(bg => bg.Genre).ToList();
                focusModel.comments = currBook.Comments.Select(c => new CommentViewModel
                {
                    UserName = c.User.UserName ?? "Unknown User",
                    Description = c.CommentContent ?? string.Empty
                }).ToList();
                focusModel.Favourites = currBook.Favourites;
                };
            return focusModel;
        }

        public async Task<BookCreationViewModel> getBookInfoAsync(int editId)
        {
            var book = await _bookRepository.GetSingleBookForEditAsync(editId);
            if(book is null)
            {
                return null;
            }

            return new BookCreationViewModel
            {
                Id = book.Id,
                Name = book.Name, 
                Category = book.Category,
                AuthorName = book.Author.Name,
                DateWritten = book.DateWritten,
                AmountInStock = book.AmountInStock, 
                CoverImage = book.CoverImage,
                Description = book.Description,
                Pages = book.Pages, 
                ReadingTime = book.ReadingTime,
                GenreOptions = await _genreRepository.GetAllGenresAsync(),
            };
        }

        public async Task<BooksFromCategoryViewModel> GetAllBooksFromSpecificCategoryAsync(int modifier)
        {
            BooksFromCategoryViewModel returnModel = new BooksFromCategoryViewModel();
            
            _logger.LogInformation("GETTING ALL BOOKS FROM A CERTAIN GENRE FILLING THE VIEW");

            returnModel.BooksFromCategory = await _bookRepository.RenderBooksByCategoryAsync(modifier);
            returnModel.Category = returnModel.BooksFromCategory.Select(rm => rm.Category).FirstOrDefault()?.ToString();

            if (returnModel.BooksFromCategory.Count == 0)
            {
                returnModel.Message = $"No books found from {returnModel.Category} category"; 
                return returnModel;
            }
           
            returnModel.Message = $"All books from {returnModel.Category} category";
            return returnModel; 
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
            return await _bookRepository.RenderBooksLeaderboardAsync();
                
        }

        public async Task<bool> CheckIfUserCantBorrowAsync(ClaimsPrincipal User)
        {
            var currUser = await _userManager.GetUserAsync(User);
            if (currUser.CantBorrow)
            {
                return true; 
            }
            else if (!currUser.CantBorrow)
            {
                return false;
            }
            return true;
        }
    }
}
