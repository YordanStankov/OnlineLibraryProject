using FInalProject.ViewModels;
using FInalProject.Data.Models;
namespace FInalProject.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<List<AdminBookListViewModel>> RenderAdminBookListAsync();
        Task<List<Book>> GetAllBooksAsync();
        Task<Book> GetSingleBookAsync(int bookId);
        Task<Book> GetSingleBookForEditAsync(int editId);
        Task<List<BookListViewModel>> RenderBookListAsync();
        Task UpdateBookAsync(Book book);
        Task AddBookAsync (Book book);
        Task<List<BookListViewModel>> RenderBooksByCategoryAsync(int modifier);
        Task<List<BooksLeaderboardViewModel>> RenderBooksLeaderboardAsync();

    }
}
