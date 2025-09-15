using FInalProject.Application.DTOs.BorrowedBook;
using FInalProject.Domain.Models;

namespace FInalProject.Application.Interfaces
{
    public interface IBorrowedBookRepository
    {
        Task<bool> GetSingleBorrowedBookAsync(string userId, int bookId);
        Task<List<BorrowedBookListDTO>> ReturnBorrowedBookListDTOAsync(string userId);
        void AddBorrowedBook(BorrowedBook newBorrow);
        Task SaveChangesAsync();
        Task TSaveChangesAsync(CancellationToken cancellationToken);
        void RemoveBorrowedBook(BorrowedBook removedBorrow);
        Task<BorrowedBook> ReturnBorrowedBookToReturnAsync(int bookId, string userId);
        Task<bool> UserHasOverdueBooksAsync(string userId);
        Task<List<BorrowedBook>> GetOverdueBooksListAsync(CancellationToken cancellationToken, DateTimeOffset currentTime);
        Task ReturnOneBookToStockAsync(int bookId);
    }
}
