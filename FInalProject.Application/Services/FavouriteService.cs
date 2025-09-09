using FInalProject.Domain.Models;
using FInalProject.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using FInalProject.Application.ViewModels.Book;

namespace FInalProject.Application.Services
{
    public interface IFavouriteService
    {
        Task UpdateFavouritesAsync(int amount, int bookId, ClaimsPrincipal user);
        Task<List<LikedBookListViewModel>> ReturnLikedBookListAsync(ClaimsPrincipal user);
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

        public async Task<List<LikedBookListViewModel>> ReturnLikedBookListAsync(ClaimsPrincipal user)
        {
            var CurrUser = await _userManager.GetUserAsync(user);
            if (CurrUser == null)
                throw new Exception("User is null when ReturingLikedBookList in FavouriteService");
            var books = await _favouriteRepository.ReturnLikedBookListAsync(CurrUser.Id);
            return books;


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
