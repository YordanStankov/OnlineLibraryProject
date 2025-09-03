using FInalProject.Application.ViewModels.Book;
using FInalProject.Domain.Models;
using System.Security.Claims;

namespace FInalProject.Application.Interfaces
{
    public interface IFavouriteRepository
    {
        Task<List<LikedBookListViewModel>> ReturnLikedBookListAsync(ClaimsPrincipal user);
        Task UpdateFavouritesAsync(int amount, int bookId, ClaimsPrincipal user);


    }
}
