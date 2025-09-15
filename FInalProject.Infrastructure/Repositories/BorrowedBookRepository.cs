using FInalProject.Domain.Models;
using FInalProject.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using FInalProject.Application.ViewModels.Book;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using FInalProject.Application.DTOs.BorrowedBook;

namespace FInalProject.Infrastructure.Repositories
{
    public class BorrowedBookRepository : IBorrowedBookRepository
    {
        private readonly ApplicationDbContext _context;
        public BorrowedBookRepository(ApplicationDbContext context) 
        { 
            _context = context;
        }

        public async Task<bool> UserHasOverdueBooksAsync(string userId)
        {
            bool hasOverdue = false;
            hasOverdue = await _context.BorrowedBooks
                  .AnyAsync(bb => bb.UserId == userId && bb.StrikeGiven);
            return hasOverdue;
        }

        public async Task<bool> GetSingleBorrowedBookAsync(string userId, int bookId)
        {
             return await _context.BorrowedBooks.AnyAsync(b => b.UserId == userId && b.BookId == bookId);
        }

        public void RemoveBorrowedBook(BorrowedBook removedBorrow)
        {
            _context.BorrowedBooks.Remove(removedBorrow);
        }
        public void AddBorrowedBook(BorrowedBook newBorrow)
        {
            _context.BorrowedBooks.Add(newBorrow);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task TSaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<BorrowedBook> ReturnBorrowedBookToReturnAsync(int bookId, string userId)
        {
            BorrowedBook borrowed = new BorrowedBook();
            borrowed = await _context.BorrowedBooks
                .Include(bb => bb.Book)
                .FirstOrDefaultAsync(bb => bb.BookId == bookId && bb.UserId == userId);
            return borrowed;
        }

        public async Task<List<BorrowedBook>> GetOverdueBooksListAsync(CancellationToken cancellationToken, DateTimeOffset currentTime)
        {
           var books = await _context.BorrowedBooks
                         .Include(bb => bb.Book)
                         .Include(bb => bb.User)
                         .Where(bb => bb.UntillReturn < currentTime && !bb.StrikeGiven)
                         .ToListAsync(cancellationToken);
            return books;
        }

        public async Task ReturnOneBookToStockAsync(int bookId)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            book.AmountInStock++;
        }

        public async Task<List<BorrowedBookListDTO>> ReturnBorrowedBookListDTOAsync(string userId)
        {

            List<BorrowedBookListDTO> borrowedBooks = new List<BorrowedBookListDTO>();
            borrowedBooks = await _context
                    .BorrowedBooks
                    .Where(bb => bb.UserId == userId)
                    .Select(bb => new BorrowedBookListDTO
                    {
                        CoverImage = bb.Book.CoverImage,
                        BookId = bb.BookId,
                        UserId = userId,
                        Name = bb.Book.Name,
                        UntillReturn = bb.UntillReturn
                    }).ToListAsync();
            return borrowedBooks;
        }
    }
}
