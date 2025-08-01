using FInalProject.Data.Models;
using FInalProject.Repositories.DataAcces;
using FInalProject.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FInalProject.Services
{
    public interface IFavouriteService
    {
        Task UpdateFavouritesAsync(int amount, int bookId, ClaimsPrincipal user);
    }
    public class FavouriteService : IFavouriteService
    {
        private readonly UserManager<User> _userManager;
        private readonly IFavouriteRepository _favouriteRepository;
        public FavouriteService(UserManager<User> userManager, IFavouriteRepository favouriteRepository)
        {
            _favouriteRepository = favouriteRepository;
            _userManager = userManager;
        }
        public async Task UpdateFavouritesAsync(int amount, int bookId, ClaimsPrincipal user)
        {
            if (user != null)
            {
                string userId = _userManager.GetUserId(user);

                var Rating = await _favouriteRepository.ReturnFavouriteEntityToUpdateAsync(bookId, userId);
                if (Rating is not null)
                {
                    if (Rating.Amount == amount)
                    {
                        Rating.Amount -= amount;
                    }
                    else
                    {
                        Rating.Amount = amount;
                    }
                    _favouriteRepository.UpdateFavourite(Rating);
                }
                else
                {
                    Rating = new Favourite
                    {
                        UserId = userId,
                        Amount = amount,
                        BookId = bookId
                    };
                    await _favouriteRepository.AddFavouriteAsync(Rating);
                }
                await _favouriteRepository.SaveChangesAsync();
            }
        }
    }
}
