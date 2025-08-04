using FInalProject.Data.Models;

namespace FInalProject.Application.Interfaces
{
    public interface IFavouriteAuthorRepository
    {
        Task AddFavouriteAuthorAsync(FavouriteAuthor favourite);
        void RemoveFavouriteAuthor(FavouriteAuthor favourite);
        Task<FavouriteAuthor> GetFavouriteAuthorAsync(int authorId, string userId);
        Task SaveChangesAsync();    
    }
}
