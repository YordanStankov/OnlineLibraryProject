using FInalProject.Data.Models;
using FInalProject.Application.ViewModels.Admin.Book;
using FInalProject.Application.ViewModels.Book;
namespace FInalProject.Application.Interfaces
{
    public interface IBookRepository
    {
        Task<List<AdminBookListViewModel>> RenderAdminBookListAsync();
        Task<List<Book>> GetAllBooksAsync();
        Task<BookFocusViewModel> GetSingleBookForFocusAsync(int bookId);
        Task<BookCreationViewModel> GetSingleBookForEditAsync(int editId);
        Task<List<BookListViewModel>> RenderBookListAsync();
        void UpdateBook(Book book);
        void AddBook(Book book);
        Task<List<BookListViewModel>> RenderBooksByCategoryAsync(int modifier);
        Task<List<BooksLeaderboardViewModel>> RenderBooksLeaderboardAsync();
        Task<Book> ReturnBookEntityToEditAsync(int bookId);
        Task<Book> ReturnBookEntityToBorrowAsync(int bookId);
        Task<Book> ReturnBookEntityToDeleteAsync(int bookId);
        void RemoveBook(Book book);
        Task SaveChangesAsync();
        Task<List<BookListViewModel>> RenderSearchedBookListAsync(string searchQuery);
    }
}
