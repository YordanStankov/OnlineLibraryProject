using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.Repositories.Interfaces;
using FInalProject.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FInalProject.Repositories.DataAcces
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;
        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<Book>> GetAllBooksAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<AdminBookListViewModel>> RenderAdminBooksInViewModelAsync()
        {
            return await _context.Books.AsNoTracking().Select(b => new AdminBookListViewModel
            {
                BookId = b.Id,
                BookName = b.Name,
                BooksBorrowed = b.BorrowedBooks.Count(),
                BookStock = b.AmountInStock,
                Category = b.CategoryString,
                genres = b.BookGenres.Select(bg => bg.Genre.Name).ToList() ?? new List<string>()
            }).ToListAsync();
        }
    }
}
