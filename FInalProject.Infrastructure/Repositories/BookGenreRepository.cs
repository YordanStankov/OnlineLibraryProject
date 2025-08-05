using FInalProject.Data.Models;
using FInalProject.Application.Interfaces;

namespace FInalProject.Infrastructure.Repositories
{
    public class BookGenreRepository : IBookGenreRepository
    {
        private readonly ApplicationDbContext _context;

        public BookGenreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddListOfNewBookGenresAsync(List<BookGenre> bookGenres)
        {
           await _context.AddRangeAsync(bookGenres);
        }

        public async Task AddNewBookGenreAsync(BookGenre bookGenre)
        {
            await _context.BookGenres.AddAsync(bookGenre);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
