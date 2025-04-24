using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FInalProject.Services
{
    public interface IGenreService 
    {
        Task<bool> DeleteGenreAsync(int doomedGenreId);
        Task<bool> AddGenreAsync(string Name);
        Task<BooksFromGenreViewModel> GetAllBooksOfCertainGenre(int genreId);
        Task<List<Genre>> GetGenreListAsync();
        Task<GenreEditViewModel> ProvideGenreForPartialAsync(int genreEditId);
        Task<bool> SaveChangesToGenreAsync(GenreEditViewModel model);
    }


    public class GenreService : IGenreService
    {
        private readonly ILogger<GenreService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public GenreService(ApplicationDbContext context, UserManager<User> userManager, ILogger<GenreService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        //Adding and deleting genres
        public async Task<bool> AddGenreAsync(string Name)
        {
            _logger.LogInformation("ADDING GENRE METHOD");
            var existingGenre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == Name);
            if (existingGenre == null)
            {
                _logger.LogInformation("ADDING NEW GENRE");
                Genre genre = new Genre
                {
                    Name = Name
                };
                await _context.Genres.AddAsync(genre);
                await _context.SaveChangesAsync();
                _logger.LogInformation("ADDED THE GENRE");
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteGenreAsync(int doomedGenreId)
        {
            _logger.LogInformation("DELETE GENRE METHOD");
            var doomedGenre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == doomedGenreId);
            if (doomedGenre != null)
            {
                _context.Remove(doomedGenre);
                await _context.SaveChangesAsync();
                _logger.LogInformation("REMOVED THE GENRE");
                return true;
            }
            return false;
        }

        //Providing lists of genres
        public async Task<List<Genre>> GetGenreListAsync()
        {
            _logger.LogInformation("GETING ALL GENRES METHOD");
            var genreList = await _context.Genres.AsNoTracking().ToListAsync();
            return genreList;
        }

        public async Task<BooksFromGenreViewModel> GetAllBooksOfCertainGenre(int genreId)
        {
            BooksFromGenreViewModel GenreBooks = new BooksFromGenreViewModel();

            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == genreId);
            GenreBooks.Genre = genre?.Name ?? "Null";

            GenreBooks.BooksMatchingGenre = await _context.BookGenres
                .AsNoTracking()
                .Where(bg => bg.GenreId == genreId && bg.Book.AmountInStock > 0)
                .Select(bg => new BookListViewModel()
                {
                    Id = bg.Book.Id,
                    AuthorName = bg.Book.Author.Name,
                    CoverImage = bg.Book.CoverImage,
                    Pages = bg.Book.Pages,
                    Name = bg.Book.Name,
                    Genres = new List<string> { bg.Genre.Name }
                }).ToListAsync();

            if (GenreBooks.BooksMatchingGenre == null || !GenreBooks.BooksMatchingGenre.Any())
            {
                GenreBooks.Message = $"No books from genre {GenreBooks.Genre}";
                return GenreBooks;
            }
            else
            {
                GenreBooks.Message = $"Books found from {GenreBooks.Genre}";
                return GenreBooks;
            }
        }

        //Genre editing
        public async Task<GenreEditViewModel> ProvideGenreForPartialAsync(int genreEditId)
        {
            var selectedGenre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == genreEditId);
            return new GenreEditViewModel
            {
                Id = selectedGenre.Id,
                Name = selectedGenre.Name
            };
        }

        public async Task<bool> SaveChangesToGenreAsync(GenreEditViewModel model)
        {
            var genreNeedsEdit = await _context.Genres.FirstOrDefaultAsync(g => g.Id == model.Id);
            if(genreNeedsEdit != null)
            {
                genreNeedsEdit.Name = model.Name;
                _logger.LogInformation("CHANGED THE NAME OF THE GENRE WOOOOOOOOOOOO");
                await _context.SaveChangesAsync();
                return true;
            }
            _logger.LogError("COULDNT FIND GENRE WHEN SAVING THE CHANGES TO IT");
            return false;
        }
    }
}
