using FInalProject.Domain.Models;
using FInalProject.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using FInalProject.Application.ViewModels.Book;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace FInalProject.Infrastructure.Repositories
{
    public class FavouriteRepository : IFavouriteRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;


        public FavouriteRepository(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task AddFavouriteAsync(Favourite favourite)
        {
            await _context.Favourites.AddAsync(favourite);
        }

        private async Task<Favourite> ReturnFavouriteEntityToUpdateAsync(int bookId, string userId)
        {
            Favourite favourite = new Favourite();
            favourite = await _context.Favourites
                .FirstOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId);
            return favourite;
        }

        public async Task<List<LikedBookListViewModel>> ReturnLikedBookListAsync(ClaimsPrincipal user)
        {
            var userId = _userManager.GetUserId(user);
            if (userId == null)
                throw new Exception("User is null in ReturnLikedBookListAsync in FavouriteRepository");
            List<LikedBookListViewModel> likedBookList = new List<LikedBookListViewModel>();
            likedBookList = await _context.Favourites
                .Where(r => r.UserId == userId)
                .Select(r => new LikedBookListViewModel
                {
                    CoverImage = r.Book.CoverImage,
                    Id = r.BookId,
                    Name = r.Book.Name
                }
                ).ToListAsync();
            return likedBookList;
        }

        

        public async Task UpdateFavouritesAsync(int amount, int bookId, ClaimsPrincipal user)
        {
            if (user != null)
            {
                string userId = _userManager.GetUserId(user);

                var Rating = await ReturnFavouriteEntityToUpdateAsync(bookId, userId);
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
                     _context.Favourites.Update(Rating);
                }
                else
                {
                    Rating = new Favourite
                    {
                        UserId = userId,
                        Amount = amount,
                        BookId = bookId
                    };
                    await _context.Favourites.AddAsync(Rating);
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}

