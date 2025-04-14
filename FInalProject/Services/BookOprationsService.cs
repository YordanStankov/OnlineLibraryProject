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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FInalProject.Services
{
    public interface IBookOprationsService
    {
        Task<bool> BorrowBookAsync(int borrowId, ClaimsPrincipal Use);
        Task<bool> EditBookAsync(BookCreationViewModel model);
        Task<List<BookListViewModel>> ReturnSearchResultsAync(string searchedString);
        Task<bool> DeleteBookAsync(int doomedId);
        Task<int> CreateCommentAsync(CreateCommentViewModel model, ClaimsPrincipal user);
        Task<bool> UpdateFavouritesAsync(int amount, int bookId, ClaimsPrincipal user);
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
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.BookGenres)
                .ThenInclude(bg => bg.Genre)
                .FirstOrDefaultAsync(b => b.Id == borrowedId);
            if(book != null)
            {
                book.AmountInStock -= 1;
                var borrowingUser = await _userManager.GetUserAsync(user);
                var borrowedBook = new BorrowedBook
                {
                    DateTaken = DateTime.Now,
                    UntillReturn = DateTime.Now.AddDays(14),
                    UserId = borrowingUser.Id,
                    Book = book,
                    BookId = book.Id,
                    User = borrowingUser
                };
                await _context.BorrowedBooks.AddAsync(borrowedBook);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
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
                _logger.LogInformation("BLITZED THE BOOK");
                return true;
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
            
           if(model.editor == 0)
            {
                bookToEdit.Name = model.Name;
                bookToEdit.AmountInStock = model.AmountInStock;
                bookToEdit.Pages = model.Pages;
                bookToEdit.Category = model.Category;
                bookToEdit.CategoryString = model.Category.ToString();
                bookToEdit.Description = model.Description;
                bookToEdit.CoverImage = model.CoverImage;
                bookToEdit.ReadingTime = model.ReadingTime;
                var searchedAuthor = await _context.Authors.FirstOrDefaultAsync(a => a.Id == bookToEdit.Author.Id);
                bookToEdit.Author = searchedAuthor ?? new Author { Name = model.AuthorName };

                var existingGenreIds = bookToEdit.BookGenres.Select(bg => bg.GenreId).ToList();
                if(existingGenreIds != null && model.SelectedGenreIds != null)
                {
                    var newGenres = model.SelectedGenreIds.Except(existingGenreIds);
                    var removedGenres = existingGenreIds.Except(model.SelectedGenreIds);
                    bookToEdit.BookGenres = bookToEdit.BookGenres.Where(bg => !removedGenres.Contains(bg.GenreId)).ToList();

                    foreach (var genreId in newGenres)
                    {
                        _context.BookGenres.Add(new BookGenre { BookId = bookToEdit.Id, GenreId = genreId });
                    }
                }
                await _context.SaveChangesAsync();
                _logger.LogInformation("EDITED BOOK BY ADMIN");
                return true;
            }
           else if(model.editor == 1)
            {
                bookToEdit.Name = model.Name;
                await _context.SaveChangesAsync();
                _logger.LogInformation("EDITED BOOK BY LIBRARIAN");
                return true;
            }
            _logger.LogError("Book editing failed");
            return false; 
        }

        public async Task<List<BookListViewModel>> ReturnSearchResultsAync(string searchedString)
        {
            if (string.IsNullOrWhiteSpace(searchedString))
            {
                return null;
            }
            string loweredSearch = searchedString.ToLower();
            var books1 = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.BookGenres)
                .ThenInclude(bg => bg.Genre)
                .Where(b => (b.Name.ToLower().Contains(loweredSearch) ||
                    b.Author != null && b.Author.Name.ToLower().Contains(loweredSearch) || 
                    b.BookGenres.Any(bg => bg.Genre.Name.ToLower().Contains(loweredSearch)) ||
                    b.CategoryString.ToLower().Contains(loweredSearch)) && b.AmountInStock > 0)
                .ToListAsync();

            return books1.Select(b => new BookListViewModel
            {
                Id = b.Id,
                Name = b.Name,
                Pages = b.Pages,
                AuthorName = b.Author.Name,
                Category = b.Category,
                CoverImage = b.CoverImage,
                Genres = b.BookGenres.Select(bg => bg.Genre.Name).ToList()
            }).ToList();
        }

        public async Task<bool> UpdateFavouritesAsync(int amount, int bookId, ClaimsPrincipal user)
        {
            var Rating = await _context.Favourites
                .FirstOrDefaultAsync(r => r.BookId == bookId && r.UserId == _userManager.GetUserId(user));
            if (Rating is not null)
            {
                if (Rating.Amount == amount)
                {
                    Rating.Amount -= amount;
                }
                else
                {
                    Rating.Amount = amount;
                }
                _context.Update(Rating);
                await _context.SaveChangesAsync();
                return true;
            }

            Rating = new Favourite
            {
                UserId = _userManager.GetUserId(user),
                Amount = amount,
                BookId = bookId
            };
            await _context.AddAsync(Rating);
            await _context.SaveChangesAsync();
            return false;
        }
    }
}

