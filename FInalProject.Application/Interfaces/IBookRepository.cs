using FInalProject.Domain.Models;
using FInalProject.Application.ViewModels.Book;
using FInalProject.Application.DTOs.Book;
using FInalProject.Application.DTOs.Admin;
namespace FInalProject.Application.Interfaces
{
    public interface IBookRepository
    {
        Task<List<Book>> GetAllBooksAsync();
        Task<List<BookListDTO>> GetAllBooksDTOAsync();
        Task<List<AdminBookListDTO>> GetAllAdminBooksDTOAsync();
        Task<BookFocusViewModel> GetSingleBookForFocusAsync(int bookId);
        Task<BookCreationViewModel> GetSingleBookForEditAsync(int editId);
        void UpdateBook(Book book);
        void AddBook(Book book);
        Task<List<BookListDTO>> ReturnBooksByCategoryDTOAsync(int modifier);
        Task<List<BooksLeaderboardDTO>> ReturnBooksLeaderboardDTOAsync();
        Task<Book> ReturnBookEntityToEditAsync(int bookId);
        Task<Book> ReturnBookEntityToBorrowAsync(int bookId);
        Task<Book> ReturnBookEntityToDeleteAsync(int bookId);
        void RemoveBook(Book book);
        Task SaveChangesAsync();
        Task<List<BookListDTO>> ReturnSearchedBooksDTOAsync(string searchQuery);
    }
}
