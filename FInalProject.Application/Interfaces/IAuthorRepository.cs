using FInalProject.Domain.Models;

namespace FInalProject.Application.Interfaces
{
    public interface IAuthorRepository
    {
        Task<Author> GetAuthorByIdAsync(int authorId);
        Task<Author> GetAuthorByNameAsync(string name);
        void AddToAuthorBookList(Author author, Book book);
        void UpdateAuthor(Author author);
        Task AddAuthorAsync(Author author);
        Task<List<Author>> ReturnAuthorListAsync();
        Task<Author> GetAuthorWithBooksByIdAsync(int authorId);
        Task<List<Author>> ReturnSearchedAuthorListAsync(string searchQuery);
        Task SaveChangesAsync();
    }
}
