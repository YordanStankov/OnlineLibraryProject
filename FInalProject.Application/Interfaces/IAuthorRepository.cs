using FInalProject.Application.DTOs.Author;
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
        Task<List<AuthorListDTO>> ReturnAuthorListDTOAsync();
        Task<Author> GetAuthorWithBooksByIdAsync(int authorId);
        Task<List<AuthorListDTO>> ReturnSearchedAuthorListDTOAsync(string searchQuery);
        Task SaveChangesAsync();
    }
}
