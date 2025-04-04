using Microsoft.AspNetCore.Mvc;
using FInalProject.Data.Models;
using FInalProject.ViewModels;
using FInalProject.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Linq;

namespace FInalProject.Services
{
    public interface IBookOprationsService
    {
        Task<bool> BorrowBookAsync(int borrowId, ClaimsPrincipal Use);
        Task<bool> EditBookAsync(BookCreationViewModel model);
        Task<List<BookListViewModel>> ReturnSearchResultsAync(string searchedString);
        Task<bool> DeleteBookAsync(int doomedId);
        Task<int> CreateCommentAsync(CreateCommentViewModel model, ClaimsPrincipal user);
    }
    public class BookOprationsService : IBookOprationsService
    {
        private readonly ILogger<BookOprationsService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public BookOprationsService(ApplicationDbContext context, UserManager<User> userManager, ILogger<BookOprationsService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<bool> BorrowBookAsync(int borrowedId, ClaimsPrincipal user)
        {
            _logger.LogInformation("BORROWING BOOK");
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == borrowedId);
            book.DateTaken = DateTime.Now;
            book.UntillReturn = DateTime.Now.AddDays(14);
            book.AmountInStock -= 1;
            var userId = _userManager.GetUserId(user);
            var borrowedBook = new BorrowedBook
            {
                UserId = userId, 
                BookId = borrowedId
            };
            await _context.BorrowedBooks.AddAsync(borrowedBook);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CreateCommentAsync(CreateCommentViewModel model, ClaimsPrincipal user)
        {
            _logger.LogInformation("CREATING COMMENT");
            var userId = _userManager.GetUserId(user);
            if(userId == null)
            {
                return -1; 
            }
            var comment = new Comment
            {
                UserId = userId, 
                BookId = model.BookId,
                CommentContent = model.Description
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return comment.BookId;
        }

        public async Task<bool> DeleteBookAsync(int doomedId)
        {
            _logger.LogInformation("DELETING BOOK");
            var doomedBook = await _context.Books.
                Include(b => b.Comments)
                .Include(b => b.BookGenres)
                .Include(b => b.Favourites)
                .FirstOrDefaultAsync(b => b.Id == doomedId); 
            if(doomedBook != null)
            {
                _context.Remove(doomedBook);
                await _context.SaveChangesAsync();
                return true;
                _logger.LogInformation("BLITZED THE BOOK");
            }
            return false;
        }

        public async Task<bool> EditBookAsync(BookCreationViewModel model)
        {
            _logger.LogInformation("EDITING BOOK");
            var bookToEdit = await _context.Books
                .Include(bte => bte.Author)
                .Include(bte => bte.BookGenres)
                .ThenInclude(bte => bte.Genre)
                .FirstOrDefaultAsync(bte => bte.Id == model.Id);
            
            if(model == null || model.SelectedGenreIds == null)
            {
                _logger.LogError("AN ERROR OCCURED IN EDITING BOOKS");
                return false;
            }

            bookToEdit.Name = model.Name;
            bookToEdit.AmountInStock = model.AmountInStock;
            bookToEdit.Pages = model.Pages;
            bookToEdit.Category = model.Category;
            bookToEdit.Description = model.Description;
            bookToEdit.CoverImage = model.CoverImage;
            bookToEdit.ReadingTime = model.ReadingTime;
            var searchedAuthor = await _context.Authors.FirstOrDefaultAsync(a => a.Id == bookToEdit.Author.Id);
            bookToEdit.Author = searchedAuthor ?? new Author { Name = model.AuthorName };

            var existingGenreIds = bookToEdit.BookGenres.Select(bg => bg.GenreId).ToList();
            var newGenres = model.SelectedGenreIds.Except(existingGenreIds);
            var removedGenres = existingGenreIds.Except(model.SelectedGenreIds);

            bookToEdit.BookGenres = bookToEdit.BookGenres.Where(bg => !removedGenres.Contains(bg.GenreId)).ToList();

            foreach (var genreId in newGenres)
            {
                _context.BookGenres.Add(new BookGenre { BookId = bookToEdit.Id, GenreId = genreId });
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("EDITED BOOK");
            return true; 
        }

        public async Task<List<BookListViewModel>> ReturnSearchResultsAync(string searchedString)
        {
            _logger.LogInformation("SEARCHING FOR BOOKS"); 
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
