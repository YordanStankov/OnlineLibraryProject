using FInalProject.Domain.Models;
using FInalProject.Application.ViewModels.Genre.GenreOprations;
using FInalProject.Application.DTOs.Book;
using FInalProject.Application.DTOs.Genre;

namespace FInalProject.Application.Interfaces
{
    public interface IGenreRepository
    {
        Task<List<GenreListDTO>> GetAllGenresDTOAsync();
        Task UpdateGenreAsync(Genre genre);
        Task<List<Genre>> GetListOfGenresAsync();
        Task<Genre> GetGenreByNameAsync(string name);
        Task SaveChangesAsync();
        Task AddGenreAsync(Genre genre);
        void RemoveGenre(Genre genre);
        Task<Genre> GetGenreByIdAsync(int id);
        Task<List<BookListDTO>> GetSpecificGenreBookListDTOAsync(int genreId);
        Task<GenreEditDTO> ReturnSingleGenreDTOToEditAsync(int id);
    }
}
