using FInalProject.Application.DTOs.AuthorDTOs;
using FInalProject.Application.ViewModels.Author;
using FInalProject.Domain.Models;

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
        Task AddPortraitToAuthorAsync(AddAuthorPortraitDTO dto);
        Task SaveChangesAsync();
        Task<AuthorProfileViewModel> RenderAuthorProfileASync(FavouriteAuthorDTO dto);
    }
}
