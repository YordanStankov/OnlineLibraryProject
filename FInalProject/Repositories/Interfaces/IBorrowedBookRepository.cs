using FInalProject.Data.Models;

namespace FInalProject.Repositories.Interfaces
{
    public interface IBorrowedBookRepository
    {
        Task<BorrowedBook> GetSingleBorrowedBookAsync(string userId, int bookId);
    }
}
