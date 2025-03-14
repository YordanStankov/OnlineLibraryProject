using FInalProject.Data;
using FInalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FInalProject.Services
{
    public interface IGenreService 
    {
        Task<int> AddGenreAsync(string Name);
        Task<List<Genre>> GetGenreListAsync();
    }


    public class GenreService : IGenreService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public GenreService(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<int> AddGenreAsync(string Name)
        {
            int newAuthorAdded = 0;
            int authorAlreadyExists = 1;
            int authorNotAdded = 2; 
            var existingGenre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == Name);
            if (existingGenre == null)
            {
                Genre genre = new Genre
                {
                    Name = Name
                };
                _context.Genres.Add(genre);
                await _context.SaveChangesAsync();
                return 0;
            }
            if(existingGenre != null)
            {
                return 1;
            }
            return 2; 
            
            
            
        }

        public async Task<List<Genre>> GetGenreListAsync()
        {
            var genreList = await _context.Genres.ToListAsync();
            return genreList;
        }
    }
}
