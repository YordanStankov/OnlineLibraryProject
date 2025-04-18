
using FInalProject.ViewModels;
using FInalProject.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using FInalProject.Data.Models;
namespace FInalProject.Services
{
    public interface IAuthorsService
    {
        Task<bool> LikeAuthorAsync();
        Task<List<AuthorListViewModel>> RenderAuthorListAsync();
        Task<AuthorProfileViewModel> RenderAuthorProfileAsync(int authorId); 
    }
    public class AuthorsService : IAuthorsService
    {
        private readonly ApplicationDbContext _context;
        public AuthorsService(ApplicationDbContext context)
        {
            _context = context;
        }
        public Task<bool> LikeAuthorAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<AuthorListViewModel>> RenderAuthorListAsync()
        {
            return await _context.Authors.Select(a => new AuthorListViewModel
            {
                AuthorId = a.Id,
                AuthorName = a.Name,
                Favourites = a.FavouriteAuthors.Count()
            }).ToListAsync();
        }
        public async Task<AuthorProfileViewModel> RenderAuthorProfileAsync(int authorId)
        {
            var author = await _context.Authors
                .AsNoTracking()
                .Include(a => a.Books)
                .ThenInclude(b => b.BookGenres)
                .ThenInclude(bg => bg.Genre)
                .Include(a => a.FavouriteAuthors)
                .FirstOrDefaultAsync(a => a.Id == authorId);
            return new AuthorProfileViewModel
            {
                AuthorName = author.Name,
                AuthorPortrait = author.Portrait,
                AuthorsBooks = author.Books.Select(n => new BookListViewModel
                {
                    Id = n.Id,
                    Name = n.Name,
                    Pages = n.Pages,
                    Category = n.Category,
                    AuthorName = n.Author.Name,
                    CoverImage = n.CoverImage,
                    Genres = n.BookGenres.Select(bg => bg.Genre.Name).ToList()
                }).ToList(),
                FavouritesCount = author.FavouriteAuthors.Count()
            };
        }

    }
}
