using FInalProject.Domain.Models;

namespace FInalProject.Application.Interfaces
{
    public interface IFavouriteRepository
    {
        void UpdateFavourite(Favourite rating);
        Task AddFavouriteAsync(Favourite favourite);
        Task SaveChangesAsync();
        Task<Favourite> ReturnFavouriteEntityToUpdateAsync(int bookId, string userId);

    }
}
