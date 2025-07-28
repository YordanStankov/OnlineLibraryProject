using FInalProject.Data.Models;

namespace FInalProject.Repositories.Interfaces
{
    public interface IBookGenreRepository
    {
        Task AddNewBookGenreAsync(BookGenre bookGenre);
        Task AddListOfNewBookGenresAsync(List<BookGenre> bookGenres);
        Task SaveChangesAsync();
    }
}
