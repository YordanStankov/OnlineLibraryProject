using FInalProject.Domain.Models;
using FInalProject.Application.ViewModels.Genre.GenreOprations;
using FInalProject.Application.ViewModels.Book;

namespace FInalProject.Application.Interfaces
{
    public interface IGenreRepository
    {
        Task<List<Genre>> GetAllGenresAsync();
        Task UpdateGenreAsync(Genre genre);
        Task<Genre> GetGenreByNameAsync(string name);
        Task SaveChangesAsync();
        Task AddGenreAsync(Genre genre);
        void RemoveGenre(Genre genre);
        Task<Genre> GetGenreByIdAsync(int id);
        Task<List<BookListViewModel>> RenderSpecificGenreBookListAsync(int genreId);
        Task<GenreEditViewModel> ReturnSingleGenreToEditAsync(int id);
    }
}
