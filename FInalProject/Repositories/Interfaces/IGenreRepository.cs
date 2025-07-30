using FInalProject.Data.Models;
using FInalProject.ViewModels;

namespace FInalProject.Repositories.Interfaces
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
