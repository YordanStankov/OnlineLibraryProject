using FInalProject.ViewModels;
using FInalProject.Data.Models;
namespace FInalProject.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<List<AdminBookListViewModel>> RenderAdminBooksInViewModelAsync();
        Task<List<Book>> GetAllBooksAsync();
    }
}
