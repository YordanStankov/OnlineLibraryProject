using FInalProject.Domain.Models;
using FInalProject.Application.Interfaces;
using FInalProject.Application.ViewModels.Book;
using FInalProject.Application.ViewModels.Genre.GenreOprations;
using Microsoft.EntityFrameworkCore;
using FInalProject.Application.ViewModels.Genre;
using FInalProject.Application.ViewModels.Book.BookFiltering;

namespace FInalProject.Infrastructure.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly ApplicationDbContext _context;

        public GenreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GenreListViewModel>> GetAllGenresAsync()
        {
            List<GenreListViewModel> genreList = new List<GenreListViewModel>();
            genreList = await _context.Genres
                .AsNoTracking()
                .Select(g => new GenreListViewModel
                {
                    Id = g.Id,
                    Name = g.Name,
                }).ToListAsync();
            return genreList;

        }

        public async Task<bool> DeleteGenreAsync(int genreId)
        {
            var genre = await GetGenreByIdAsync(genreId);
            if (genre == null) 
                return false;

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
            return true;

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

        public async Task<List<Genre>> GetListOfGenresAsync()
        {
            return await _context.Genres.AsNoTracking().ToListAsync();
        }

        public async Task<bool> AddGenreAsync(string name)
        {
            var existingGenre = await _context.Genres
                .AsNoTracking()
                .AnyAsync(g => g.Name == name);
            if (existingGenre == false)
            {
                Genre genre = new Genre
                {
                    Name = name
                };
                await _context.Genres.AddAsync(genre);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        
        private async Task<Genre> GetGenreByIdAsync(int id)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);
            if (genre == null)
                return null;
            return genre;
        }

        private async Task<Genre> GetGenreByNameAsync(string name)
        {
            string loweredName = name.ToLower();
            var genre = await _context.Genres
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Name.ToLower() == loweredName);
            if(genre == null)
                return null;    
            return genre;
        }

        public Task<BooksFromGenreViewModel> GetAllBooksOfCertainGenreAsync(int genreId)
        {
            var books =  _context.Genres
                .AsNoTracking()
                .Where(g => g.Id == genreId)
                .Select(g => new BooksFromGenreViewModel
                {
                    Genre = g.Name,
                    BooksMatchingGenre = g.BookGenres
                        .Where(bg => bg.Book.AmountInStock > 0)
                        .Select(bg => new BookListViewModel
                        {
                            Id = bg.Book.Id,
                            AuthorName = bg.Book.Author.Name,
                            CoverImage = bg.Book.CoverImage,
                            Pages = bg.Book.Pages,
                            Name = bg.Book.Name,
                            DateWritten = bg.Book.DateWritten,
                            Genres = bg.Book.BookGenres.Select(bgg => bgg.Genre.Name).ToList()
                        }).ToList()
                }).FirstOrDefaultAsync();
            return books;
        }

        public async Task<bool> SaveChangesToGenreAsync(GenreEditViewModel model)
        {
            var genreNeedsEdit = await GetGenreByIdAsync(model.Id);
            if (genreNeedsEdit != null)
            {
                genreNeedsEdit.Name = model.Name;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
