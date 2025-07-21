using FInalProject.ViewModels;
using FInalProject.Data.Models;
namespace FInalProject.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<List<AdminBookListViewModel>> RenderAdminBooksInViewModelAsync();
        Task<List<Book>> GetAllBooksAsync();
        Task<Book> GetSingleBookAsync(int bookId);
        Task<Book> GetSingleBookForEditAsync(int editId);
        Task<List<BookListViewModel>> RenderBookListViewModelAsync();
        Task UpdateBookAsync(Book book);
        Task AddBookAsync (Book book);
    }
}
