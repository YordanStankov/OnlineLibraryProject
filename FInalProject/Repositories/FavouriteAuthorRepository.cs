using FInalProject.Data;
using FInalProject.Data.Models;
using FInalProject.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FInalProject.Repositories
{
    public class FavouriteAuthorRepository : IFavouriteAuthorRepository
    {
        private readonly ApplicationDbContext _context;

        public FavouriteAuthorRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddFavouriteAuthorAsync(FavouriteAuthor favourite)
        {
          await _context.FavouriteAuthors.AddAsync(favourite);   
        }

        public async Task<FavouriteAuthor> GetFavouriteAuthorAsync(int authorId, string userId)
        {
            FavouriteAuthor favourite = new FavouriteAuthor();
            favourite = await _context.FavouriteAuthors
                .FirstOrDefaultAsync(fa => fa.AuthorId == authorId && fa.UserId == userId);
            return favourite;
        }

        public void RemoveFavouriteAuthor(FavouriteAuthor favourite)
        {
           _context.FavouriteAuthors.Remove(favourite);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
