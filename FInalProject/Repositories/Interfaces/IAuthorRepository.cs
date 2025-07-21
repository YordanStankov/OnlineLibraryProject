using FInalProject.Data.Models;

namespace FInalProject.Repositories.Interfaces
{
    public interface IAuthorRepository
    {
        Task<Author> GetAuthorByIdAsync(int authorId);
        Task<Author> GetAuthorByNameAsync(string name);
        Task AddToAuhtorBookListAsync(Author author, Book book);
    }
}
