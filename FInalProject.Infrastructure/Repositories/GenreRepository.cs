using FInalProject.Application.DTOs.Book;
using FInalProject.Application.DTOs.Genre;
using FInalProject.Application.Interfaces;
using FInalProject.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FInalProject.Infrastructure.Repositories
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

        public async Task<List<GenreListDTO>> GetAllGenresDTOAsync()
        {
            List<GenreListDTO> genreList = new List<GenreListDTO>();
            genreList = await _context.Genres
                .AsNoTracking()
                .Select(g => new GenreListDTO
                {
                    Id = g.Id,
                    Name = g.Name,
                }).ToListAsync();
            return genreList;

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

        public async Task<List<BookListDTO>> GetSpecificGenreBookListDTOAsync(int genreId)
        {
            List<BookListDTO> list = new List<BookListDTO>();
            list = await _context.BookGenres
                .AsNoTracking()
                .Where(bg => bg.GenreId == genreId && bg.Book.AmountInStock > 0)
                .Select(bg => new BookListDTO()
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

        public async Task<GenreEditDTO> ReturnSingleGenreDTOToEditAsync(int id)
        {
            var genre = await _context.Genres.Select(g => new GenreEditDTO
            {
                Id = g.Id,
                Name = g.Name
            }).FirstOrDefaultAsync(g => g.Id == id);
            return genre;
        }

        public async Task<List<Genre>> GetListOfGenresAsync()
        {
            return await _context.Genres.AsNoTracking().ToListAsync();
        }
    }
}
