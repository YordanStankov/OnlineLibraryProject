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

        public async Task AddBookAsync(Book book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _context.Books.AsNoTracking().ToListAsync();
        }

        public async Task<Book> GetSingleBook(int BookId)
        {
            return await _context.Books.AsNoTracking().FirstOrDefaultAsync( b => b.Id == BookId);
        }

        public Task<Book> GetSingleBookAsync(int bookId)
        {
            throw new NotImplementedException();
        }

        public async Task<Book> GetSingleBookForEditAsync(int editId)
        {
            return await _context.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.BookGenres)
                .ThenInclude(b => b.Genre)
                .FirstOrDefaultAsync(b => b.Id == editId);
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

        public async Task<List<BookListViewModel>> RenderBookListViewModelAsync()
        {
            return await _context.Books
               .AsNoTracking()
               .Include(a => a.Author)
               .Include(bg => bg.BookGenres)
               .ThenInclude(g => g.Genre)
               .Where(b => b.AmountInStock > 0)
               .Select(n => new BookListViewModel()
               {
                   Id = n.Id,
                   Name = n.Name,
                   Pages = n.Pages,
                   Category = n.Category,
                   AuthorName = n.Author.Name,
                   DateWritten = n.DateWritten,
                   CoverImage = n.CoverImage,
                   Genres = n.BookGenres.Select(bg => bg.Genre.Name).ToList(),
               }).ToListAsync();
        }

        public async Task UpdateBookAsync(Book book)
        {
             _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }
    }
}
