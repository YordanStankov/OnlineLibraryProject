using FInalProject.Domain.Models;
using FInalProject.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using FInalProject.Application.ViewModels.Book;
using FInalProject.Application.DTOs.Book;

namespace FInalProject.Infrastructure.Repositories
{
    public class FavouriteRepository : IFavouriteRepository
    {
        private readonly ApplicationDbContext _context;

        public FavouriteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddFavouriteAsync(Favourite favourite)
        {
            await _context.Favourites.AddAsync(favourite);
        }

        public async Task<Favourite> ReturnFavouriteEntityToUpdateAsync(int bookId, string userId)
        {
            Favourite favourite = new Favourite();
            favourite = await _context.Favourites
                .FirstOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId);
            return favourite;
        }

        public async Task<List<LikedBookListDTO>> ReturnLikedBookListDTOAsync(string userId)
        {
            List<LikedBookListDTO> likedBookList = new List<LikedBookListDTO>();
            likedBookList = await _context.Favourites
                .Where(r => r.UserId == userId)
                .Select(r => new LikedBookListDTO
                {
                    CoverImage = r.Book.CoverImage,
                    Id = r.BookId,
                    Name = r.Book.Name
                }
                ).ToListAsync();
            return likedBookList;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void UpdateFavourite(Favourite rating)
        {
             _context.Favourites.Update(rating);
        }
    }
}
