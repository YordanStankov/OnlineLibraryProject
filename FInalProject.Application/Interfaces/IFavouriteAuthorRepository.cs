using FInalProject.Application.DTOs.AuthorDTOs;
using FInalProject.Domain.Models;
using System.Security.Claims;

namespace FInalProject.Application.Interfaces
{
    public interface IFavouriteAuthorRepository
    {
        Task AddFavouriteAuthorAsync(FavouriteAuthor favourite);
        void RemoveFavouriteAuthor(FavouriteAuthor favourite);
        Task<FavouriteAuthor> GetFavouriteAuthorAsync(int authorId, string userId);
        Task SaveChangesAsync();   
        Task<bool> AddNewFavouriteAuthorAsync(FavouriteAuthorDTO dto);

    }
}
