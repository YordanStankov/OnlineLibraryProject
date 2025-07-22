using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FInalProject.Repositories.DataAcces
{
    public class BorrowedBookRepository : IBorrowedBookRepository
    {
        private readonly ApplicationDbContext _context;
        public BorrowedBookRepository(ApplicationDbContext context) 
        { 
            _context = context;
        }
        public async Task<BorrowedBook> GetSingleBorrowedBookAsync(string userId, int bookId)
        {
            return await  _context.BorrowedBooks.FirstOrDefaultAsync(b => b.UserId == userId && b.BookId == bookId);
        }
    }
}
