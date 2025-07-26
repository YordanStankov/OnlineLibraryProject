using FInalProject.Data.Models;

namespace FInalProject.Repositories.Interfaces
{
    public interface IBorrowedBookRepository
    {
        Task<bool> GetSingleBorrowedBookAsync(string userId, int bookId);
        void AddBorrowedBook(BorrowedBook newBorrow);
        Task SaveChangesAsync();    
        void RemoveBorrowedBook(BorrowedBook removedBorrow);
        Task<BorrowedBook> ReturnBorrowedBookToReturnAsync(int bookId, string userId);
        Task<bool> UserHasOverdueBooksAsync(string userId);
    }
}
