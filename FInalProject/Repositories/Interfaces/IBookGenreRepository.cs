using FInalProject.Data.Models;

namespace FInalProject.Repositories.Interfaces
{
    public interface IBookGenreRepository
    {
        Task AddNewBookGenre(BookGenre bookGenre);
    }
}
