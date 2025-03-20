using Microsoft.AspNetCore.Mvc;
using FInalProject.Models;
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
        Task<bool> EditBookAsync(BookCreationViewModel model);
        Task<List<BookListViewModel>> ReturnSearchResultsAync(string searchedString);
        Task<bool> DeleteBookAsync(int doomedId);
        Task<int> CreateCommentAsync(CreateCommentViewModel model, ClaimsPrincipal user);
    }
    public class BookOprationsService : IBookOprationsService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public BookOprationsService(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<int> CreateCommentAsync(CreateCommentViewModel model, ClaimsPrincipal user)
        {
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
            }
            return false;
        }

        public async Task<bool> EditBookAsync(BookCreationViewModel model)
        {
            var bookToEdit = await _context.Books
                .Include(bte => bte.Author)
                .Include(bte => bte.BookGenres)
                .ThenInclude(bte => bte.Genre)
                .FirstOrDefaultAsync(bte => bte.Id == model.Id);
            
            if(model == null || model.SelectedGenreIds == null)
            {
                return false;
            }

            bookToEdit.Name = model.Name;
            bookToEdit.AmountInStock = model.AmountInStock;
            bookToEdit.Pages = model.Pages;
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
            return true; 
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
