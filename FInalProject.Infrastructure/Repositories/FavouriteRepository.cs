using FInalProject.Domain.Models;
using FInalProject.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

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
