using FInalProject.Data.Models;

namespace FInalProject.Repositories.Interfaces
{
    public interface IGenreRepository
    {
        Task<List<Genre>> GetAllGenresAsync();
        Task UpdateGenreAsync(Genre genre);
    }
}
