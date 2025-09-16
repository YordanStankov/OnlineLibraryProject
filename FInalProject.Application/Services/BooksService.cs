using FInalProject.Domain.Models;
using FInalProject.Application.Interfaces;
using FInalProject.Application.ViewModels.Book;
using FInalProject.Application.ViewModels.Book.BookFiltering;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using FInalProject.Application.ViewModels.Genre;

namespace FInalProject.Application.Services
{
    public interface IBooksService
    {
        Task<BooksFromCategoryViewModel> GetAllBooksFromSpecificCategoryAsync(int modifier);
        Task<BookCreationViewModel> getBookInfoAsync(int editId);
        Task<List<BookListViewModel>> GetAllBooksAsync();
        Task<List<BorrowedBookListViewModel>> ReturnBorrowedBookListAsync(ClaimsPrincipal user);
        Task<BookFocusViewModel> GetBookFocusAsync(int id, ClaimsPrincipal User);
        Task<BookCreationViewModel> GetBookCreationViewModelAsync();
        Task<bool> UserRoleCheckAsync(ClaimsPrincipal user);
        Task<List<BooksLeaderboardViewModel>> ReturnLeaderboardResultsAsync();
        Task<bool> CheckIfUserCantBorrowAsync(ClaimsPrincipal User);
    }
    public class BooksService : IBooksService
    {
        public readonly ILogger<BooksService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IBookRepository _bookRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IBorrowedBookRepository _borrowedBookRepository;
        

        public BooksService(UserManager<User> userManager, ILogger<BooksService> logger, IBookRepository bookRepository, IGenreRepository genreRepository, IBorrowedBookRepository borrowedBookRepository )
        {
            _userManager = userManager;
            _logger = logger;
            _bookRepository = bookRepository;
            _genreRepository = genreRepository;
            _borrowedBookRepository = borrowedBookRepository;
        }

        public async Task<List<BookListViewModel>> GetAllBooksAsync()
        {
            _logger.LogInformation("GETTING ALL BOOKS");
           var books = await _bookRepository.GetAllBooksDTOAsync();
            return books.Select(b => new BookListViewModel
            {
                Id = b.Id,
                Name = b.Name,
                DateWritten = b.DateWritten,
                CoverImage = b.CoverImage,
                AuthorName = b.AuthorName,
                Category = b.Category,
                Genres = b.Genres
            }).ToList();
        }
        
        public async Task<BookCreationViewModel> GetBookCreationViewModelAsync()
        {
            _logger.LogInformation("FILLING THE VIEW WITH GENRE OPTIONS");
            var genres = await _genreRepository.GetAllGenresDTOAsync();
            return new BookCreationViewModel
            {
                GenreOptions = (ICollection<GenreListViewModel>)genres.Select(g => new GenreListViewModel
                {
                    Id = g.Id,
                    Name = g.Name
                })
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
            var currBook = await _bookRepository.GetSingleBookDTOForFocusAsync(id);
            if (currBook == null)
                return focusModel;
            else
            {
                focusModel = new BookFocusViewModel
                {
                    BookId = currBook.BookId,
                    BookName = currBook.BookName,
                    BookAuthorName = currBook.BookAuthorName,
                    DateWritten = currBook.DateWritten,
                    BookCover = currBook.BookCover,
                    BookPages = currBook.BookPages,
                    BookReadingTime = currBook.BookReadingTime,
                    Description = currBook.Description,
                    Category = currBook.Category,
                    AmountInStock = currBook.AmountInStock,
                    genres = currBook.genres,
                    Rating = currBook.Rating,
                    comments = currBook.comments
                };
                var borrow = await _borrowedBookRepository.GetSingleBorrowedBookAsync(curr, id);
                if (borrow == false)
                    focusModel.Borrowed = false;
                else
                    focusModel.Borrowed = true;
            }
            return focusModel;
        }

        public async Task<BookCreationViewModel> getBookInfoAsync(int editId)
        {
            var genres = await _genreRepository.GetAllGenresDTOAsync();
            var book = await _bookRepository.GetSingleBookDTOForEditAsync(editId);
            if (book is null)
                return null;
            else
            {
                var toBeReturned = new BookCreationViewModel
                {
                    Id = book.Id,
                    Name = book.Name,
                    AuthorName = book.AuthorName,
                    DateWritten = book.DateWritten,
                    Pages = book.Pages,
                    CoverImage = book.CoverImage,
                    ReadingTime = book.ReadingTime,
                    Description = book.Description,
                    AmountInStock = book.AmountInStock,
                    Category = book.Category,
                    SelectedGenreIds = book.SelectedGenreIds,
                    GenreOptions = genres.Select(g => new GenreListViewModel
                    {
                        Id = g.Id,
                        Name = g.Name
                    }).ToList()
                };
                return toBeReturned;
            }
        }

        public async Task<BooksFromCategoryViewModel> GetAllBooksFromSpecificCategoryAsync(int modifier)
        {
            BooksFromCategoryViewModel returnModel = new BooksFromCategoryViewModel();
            
            _logger.LogInformation("GETTING ALL BOOKS FROM A CERTAIN GENRE FILLING THE VIEW");

            var books = await _bookRepository.ReturnBooksByCategoryDTOAsync(modifier);
            returnModel.BooksFromCategory = books.Select(b => new BookListViewModel
            {
                Id = b.Id,
                Name = b.Name,
                DateWritten = b.DateWritten,
                CoverImage = b.CoverImage,
                AuthorName = b.AuthorName,
                Category = b.Category,
                Genres = b.Genres
            }).ToList();
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
            var books = await _bookRepository.ReturnBooksLeaderboardDTOAsync();
            return books.Select(b => new BooksLeaderboardViewModel
            {
                BookId = b.BookId,
                BookName = b.BookName,
                AuthorName = b.AuthorName,
                CategoryString = b.CategoryString,
                CommunityRating = b.CommunityRating
            }).ToList();

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

        public async Task<List<BorrowedBookListViewModel>> ReturnBorrowedBookListAsync(ClaimsPrincipal user)
        {
            var CurrUser = await _userManager.GetUserAsync(user);
            if(CurrUser == null)
            {
                throw new Exception("User is null when returning borrowedBooksViewModel");
            }
            var books = await _borrowedBookRepository.ReturnBorrowedBookListDTOAsync(CurrUser.Id);
            return books.Select(b => new BorrowedBookListViewModel
            {
                BookId = b.BookId,
                UserId = b.UserId,
                Name = b.Name,
                UntillReturn = b.UntillReturn,
                CoverImage = b.CoverImage
            }).ToList();
        }
    }
}
