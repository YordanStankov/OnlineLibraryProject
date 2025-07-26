using FInalProject.ViewModels;
using FInalProject.Data.Models;
namespace FInalProject.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<List<AdminBookListViewModel>> RenderAdminBookListAsync();
        Task<List<Book>> GetAllBooksAsync();
        Task<BookFocusViewModel> GetSingleBookForFocusAsync(int bookId);
        Task<BookCreationViewModel> GetSingleBookForEditAsync(int editId);
        Task<List<BookListViewModel>> RenderBookListAsync();
        Task UpdateBookAsync(Book book);
        Task AddBookAsync (Book book);
        Task<List<BookListViewModel>> RenderBooksByCategoryAsync(int modifier);
        Task<List<BooksLeaderboardViewModel>> RenderBooksLeaderboardAsync();
        Task<Book> ReturnBookEntityToEditAsync(int bookId);
        Task<Book> ReturnBookEntityToBorrowAsync(int bookId);
        Task<Book> ReturnBookEntityToDeleteAsync(int bookId);
        Task RemoveBookAsync(Book book);
        Task<List<BookListViewModel>> RenderSearchedBookListAsync(string searchQuery);
    }
}
