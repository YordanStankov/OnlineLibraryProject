using FInalProject.Application.Interfaces;
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
        private readonly IFavouriteRepository _favouriteRepository;
        public FavouriteService(IFavouriteRepository favouriteRepository)
        {
            _favouriteRepository = favouriteRepository;
        }

        public async Task<List<LikedBookListViewModel>> ReturnLikedBookListAsync(ClaimsPrincipal user)
        {
            
            if (user == null)
                throw new Exception("User is null when ReturingLikedBookList in FavouriteService");
            var books = await _favouriteRepository.ReturnLikedBookListAsync(user);
            return books;
        }

        public async Task UpdateFavouritesAsync(int amount, int bookId, ClaimsPrincipal user)
        {
            await _favouriteRepository.UpdateFavouritesAsync(amount, bookId, user);
        }
    }
}
