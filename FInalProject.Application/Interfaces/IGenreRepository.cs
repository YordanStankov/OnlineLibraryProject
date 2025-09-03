using FInalProject.Domain.Models;
using FInalProject.Application.ViewModels.Genre.GenreOprations;
using FInalProject.Application.ViewModels.Book;
using FInalProject.Application.ViewModels.Genre;
using FInalProject.Application.ViewModels.Book.BookFiltering;

namespace FInalProject.Application.Interfaces
{
    public interface IGenreRepository
    {
        Task<List<GenreListViewModel>> GetAllGenresAsync();
        Task<List<Genre>> GetListOfGenresAsync();
        Task<List<BookListViewModel>> RenderSpecificGenreBookListAsync(int genreId);
        Task<GenreEditViewModel> ReturnSingleGenreToEditAsync(int id);
        Task<bool> AddGenreAsync(string name);
        Task<bool> DeleteGenreAsync(int genreId);
        Task<bool> SaveChangesToGenreAsync(GenreEditViewModel model);
        Task<BooksFromGenreViewModel> GetAllBooksOfCertainGenreAsync(int genreId);


    }
}
