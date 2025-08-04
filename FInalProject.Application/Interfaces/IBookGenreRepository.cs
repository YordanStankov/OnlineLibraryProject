using FInalProject.Data.Models;

namespace FInalProject.Application.Interfaces
{
    public interface IBookGenreRepository
    {
        Task AddNewBookGenreAsync(BookGenre bookGenre);
        Task AddListOfNewBookGenresAsync(List<BookGenre> bookGenres);
        Task SaveChangesAsync();
    }
}
