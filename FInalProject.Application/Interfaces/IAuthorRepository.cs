using FInalProject.Data.Models;
using FInalProject.Application.ViewModels.Author;

namespace FInalProject.Application.Interfaces
{
    public interface IAuthorRepository
    {
        Task<Author> GetAuthorByIdAsync(int authorId);
        Task<Author> GetAuthorByNameAsync(string name);
        void AddToAuhtorBookList(Author author, Book book);
        void UpdateAuthor(Author author);
        Task<List<AuthorListViewModel>> RenderAuthorListAsync();
        Task<Author> GetAuthorWithBooksByIdAsync(int authorId);
        Task<List<AuthorListViewModel>> RenderAuthorSearchResutlsAsync(string searchQuery);
        Task SaveChangesAsync();
    }
}
