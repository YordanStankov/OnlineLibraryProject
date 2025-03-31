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
        Task<List<BookListViewModel>> GetAllBooksOfCertainGenre(int genreId);
        Task<bool> AddGenreAsync(string Name);
        Task<List<Genre>> GetGenreListAsync();
        Task<Genre> EditGenreAsync(int genreEditId);
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
            if(doomedGenre != null)
            {
                _context.Remove(doomedGenre);
                await _context.SaveChangesAsync();
                _logger.LogInformation("REMOVED THE GENRE");
                return true;
            }
            return false; 
        }

        public Task<Genre> EditGenreAsync(int genreEditId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BookListViewModel>> GetAllBooksOfCertainGenre(int genreId)
        {
            _logger.LogInformation("GETTING ALL BOOKS FROM CERTAIN GENRE METHOD");
            var correctBooks = await _context.BookGenres
                .AsNoTracking()
                .Where(bg => bg.GenreId == genreId)
                .Select(bg => new BookListViewModel()
                {
                    Id = bg.Book.Id,
                    AuthorName = bg.Book.Author.Name,
                    CoverImage = bg.Book.CoverImage,
                    Pages = bg.Book.Pages,
                    Name = bg.Book.Name,
                    Genres = new List<string> { bg.Genre.Name}
                }).ToListAsync();
            if(correctBooks == null)
            {
                _logger.LogInformation("NO BOOKS FROM THIS GENRE");
                return null; 
            }
            else
            {
                _logger.LogInformation("ALL BOOKS FROM THE CORRECT GENRE RETURNED");
                return correctBooks; 
            }
        }


        public async Task<List<Genre>> GetGenreListAsync()
        {
            _logger.LogInformation("GETING ALL GENRES METHOD");
            var genreList = await _context.Genres.AsNoTracking().ToListAsync();
            return genreList;
        }
    }
}
