using FInalProject.Data.Models;
using FInalProject.ViewModels;

namespace FInalProject.Repositories.Interfaces
{
    public interface IAuthorRepository
    {
        Task<Author> GetAuthorByIdAsync(int authorId);
        Task<Author> GetAuthorByNameAsync(string name);
        void AddToAuhtorBookList(Author author, Book book);
        void UpdateAuthor(Author author);
        Task<List<AuthorListViewModel>> RenderAuthorListAsync();
        Task SaveChangesAsync();
    }
}
