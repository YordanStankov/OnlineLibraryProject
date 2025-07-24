using FInalProject.Data.Models;

namespace FInalProject.Repositories.Interfaces
{
    public interface IBorrowedBookRepository
    {
        Task<bool> GetSingleBorrowedBookAsync(string userId, int bookId);
    }
}
