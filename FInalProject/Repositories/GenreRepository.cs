using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.Application.Interfaces;
using FInalProject.Application.ViewModels.Book;
using FInalProject.Application.ViewModels.Genre.GenreOprations;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FInalProject.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly ApplicationDbContext _context;

        public GenreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddGenreAsync(Genre genre)
        {
            await _context.Genres.AddAsync(genre);
        }
        public void RemoveGenre(Genre genre)
        {
            _context.Genres.Remove(genre);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGenreAsync(Genre genre)
        {
            _context.Genres.Update(genre);
        }

        public async Task<List<Genre>> GetAllGenresAsync()
        {
            return await _context.Genres.AsNoTracking().ToListAsync();
        }

        public async Task<Genre> GetGenreByIdAsync(int id)
        {
            Genre doomedGenre = null;
            doomedGenre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);
            return doomedGenre;
        }

        public async Task<Genre> GetGenreByNameAsync(string name)
        {
            string loweredName = name.ToLower();
            Genre genre = null;
            genre = await _context.Genres
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Name.ToLower() == loweredName);
            return genre; 
        }

        public async Task<List<BookListViewModel>> RenderSpecificGenreBookListAsync(int genreId)
        {
            List<BookListViewModel> list = new List<BookListViewModel>();
            list = await  _context.BookGenres
                .AsNoTracking()
                .Where(bg => bg.GenreId == genreId && bg.Book.AmountInStock > 0)
                .Select(bg => new BookListViewModel()
                {
                    Id = bg.Book.Id,
                    AuthorName = bg.Book.Author.Name,
                    CoverImage = bg.Book.CoverImage,
                    Pages = bg.Book.Pages,
                    Name = bg.Book.Name,
                    DateWritten = bg.Book.DateWritten,
                    Genres = new List<string> { bg.Genre.Name }
                }).ToListAsync();
            return list;
        }

        public async Task<GenreEditViewModel> ReturnSingleGenreToEditAsync(int id)
        {
            var genre = await _context.Genres.Select(g => new GenreEditViewModel
            {
                Id = g.Id,
                Name = g.Name
            }).FirstOrDefaultAsync(g => g.Id == id);
            return genre;
        }
    }
}
