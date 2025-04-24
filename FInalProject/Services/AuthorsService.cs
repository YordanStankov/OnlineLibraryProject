
using FInalProject.ViewModels;
using FInalProject.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using FInalProject.Data.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
namespace FInalProject.Services
{
    public interface IAuthorsService
    {
        Task<bool> FavouriteAuthorAsync(int authorId, ClaimsPrincipal User);
        Task<bool> AddPortraitToAuthorAsync(AddAuthorPortraitViewModel model);
        Task<List<AuthorListViewModel>> RenderAuthorListAsync();
        Task<AuthorProfileViewModel> RenderAuthorProfileAsync(int authorId, ClaimsPrincipal User);
        Task<AuthorSearchResultsViewModel> RenderSearchResultsAsync(string searchString);
    }
    public class AuthorsService : IAuthorsService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public AuthorsService(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> AddPortraitToAuthorAsync(AddAuthorPortraitViewModel model)
        {
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == model.Id);
            if (author == null) return false;
            author.Portrait = model.Picture;
            _context.Update(author);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> FavouriteAuthorAsync(int authorId, ClaimsPrincipal User)
        {
            var currUser = await _userManager.GetUserAsync(User);
            if (currUser == null) return false;
            
                var favourite = await _context.FavouriteAuthors
                 .Include(fa => fa.Author)
                .FirstOrDefaultAsync(a => a.AuthorId == authorId && a.UserId == currUser.Id);
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == authorId);
                if(favourite == null)
                {
                    FavouriteAuthor newFave = new FavouriteAuthor
                    {
                        User = currUser,
                        UserId = currUser.Id,
                        Author = author,
                        AuthorId = authorId
                    };
                    await _context.AddAsync(newFave);
                    await _context.SaveChangesAsync();
                    return true;
                }
                _context.Remove(favourite);
               await _context.SaveChangesAsync();
               return true;
        }

        public async Task<List<AuthorListViewModel>> RenderAuthorListAsync()
        {
            return await _context.Authors.Select(a => new AuthorListViewModel
            {
                AuthorId = a.Id,
                AuthorPortrait = a.Portrait,
                AuthorName = a.Name,
                Favourites = a.FavouriteAuthors.Count()
            }).ToListAsync();
        }
        public async Task<AuthorProfileViewModel> RenderAuthorProfileAsync(int authorId, ClaimsPrincipal User)
        {
            var empty = new AuthorProfileViewModel();
            var currUserId = _userManager.GetUserId(User);
            if (currUserId == null) return empty;

            var author = await _context.Authors
                .AsNoTracking()
                .Include(a => a.Books)
                    .ThenInclude(b => b.BookGenres)
                        .ThenInclude(bg => bg.Genre)
                .Include(a => a.Books)
                .Include(a => a.FavouriteAuthors)
                .FirstOrDefaultAsync(a => a.Id == authorId);

            if (author is null) return empty;

            var fave = author.FavouriteAuthors.Any(fa => fa.AuthorId == authorId && fa.UserId == currUserId);

            return new AuthorProfileViewModel
            {
                isAuthorFavourited = fave,
                AuthorId = author.Id,
                AuthorName = author.Name,
                AuthorPortrait = author.Portrait,
                AuthorsBooks = author.Books!.Select(n => new BookListViewModel
                {
                    Id = n.Id,
                    Name = n.Name!,
                    Pages = n.Pages,
                    Category = n.Category,
                    AuthorName = n.Author?.Name ?? "Unknown",
                    CoverImage = n.CoverImage!,
                    Genres = n.BookGenres?
                                .Where(bg => bg.Genre != null)
                                .Select(bg => bg.Genre!.Name!)
                                .ToList() ?? new List<string>()
                }).ToList(),
                FavouritesCount = author.FavouriteAuthors?.Count() ?? 0
            };
        }


        public async Task<AuthorSearchResultsViewModel> RenderSearchResultsAsync(string searchString)
        {
            AuthorSearchResultsViewModel results = new AuthorSearchResultsViewModel();
            results.SearchQuery = searchString;
            if (string.IsNullOrWhiteSpace(searchString))
            {
                results.Message = $"Empty search";
                return results;
            }
                results.authorsFound = await _context.Authors
                    .Where(a => a.Name.ToLower().Contains(results.SearchQuery.ToLower()))
                    .Select(a => new AuthorListViewModel
                {
                    AuthorId = a.Id,
                    AuthorPortrait = a.Portrait,
                    AuthorName = a.Name,
                    Favourites = a.FavouriteAuthors.Count()
                }).ToListAsync();
            if (!results.authorsFound.Any())
            {
                results.Message = $"No authors found with search: {results.SearchQuery}";
                return results;
            }
            results.Message = $"Authors found with search: {results.SearchQuery}";
            return results;
        }
    }
}
