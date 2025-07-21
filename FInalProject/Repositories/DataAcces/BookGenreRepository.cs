using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.Repositories.Interfaces;

namespace FInalProject.Repositories.DataAcces
{
    public class BookGenreRepository : IBookGenreRepository
    {
        private readonly ApplicationDbContext _context;

        public BookGenreRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddNewBookGenre(BookGenre bookGenre)
        {
            await _context.BookGenres.AddAsync(bookGenre);
            await _context.SaveChangesAsync();
        }
    }
}
