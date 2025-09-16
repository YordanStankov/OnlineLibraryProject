using FInalProject.Domain.Models;
using FInalProject.Application.ViewModels.Genre.GenreOprations;
using FInalProject.Application.ViewModels.Genre;
using FInalProject.Application.DTOs.Book;

namespace FInalProject.Application.Interfaces
{
    public interface IGenreRepository
    {
        Task<List<GenreListViewModel>> GetAllGenresAsync();
        Task UpdateGenreAsync(Genre genre);
        Task<List<Genre>> GetListOfGenresAsync();
        Task<Genre> GetGenreByNameAsync(string name);
        Task SaveChangesAsync();
        Task AddGenreAsync(Genre genre);
        void RemoveGenre(Genre genre);
        Task<Genre> GetGenreByIdAsync(int id);
        Task<List<BookListDTO>> GetSpecificGenreBookListDTOAsync(int genreId);
        Task<GenreEditViewModel> ReturnSingleGenreToEditAsync(int id);
    }
}
