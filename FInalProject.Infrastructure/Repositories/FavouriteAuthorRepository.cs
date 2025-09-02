using FInalProject.Application.DTOs.AuthorDTOs;
using FInalProject.Application.Interfaces;
using FInalProject.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FInalProject.Infrastructure.Repositories
{
    public class FavouriteAuthorRepository : IFavouriteAuthorRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public FavouriteAuthorRepository(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<bool> AddNewFavouriteAuthorAsync(FavouriteAuthorDTO dto)
        {
            var currUser = await _userManager.GetUserAsync(dto.UserClaim);
            if (currUser == null) return false;

            var favourite = await _context.FavouriteAuthors
                .FirstOrDefaultAsync(fa => fa.UserId == currUser.Id && fa.AuthorId == dto.Id);
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == dto.Id);
            if (favourite == null)
            {
                FavouriteAuthor newFave = new FavouriteAuthor
                {
                    User = currUser,
                    UserId = currUser.Id,
                    Author = author,
                    AuthorId = dto.Id
                };
                await AddFavouriteAuthorAsync(newFave);
                await SaveChangesAsync();
                return true;
            }
            RemoveFavouriteAuthor(favourite);
            await SaveChangesAsync();
            return true;
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
