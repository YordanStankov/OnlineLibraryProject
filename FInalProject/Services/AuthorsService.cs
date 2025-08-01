using FInalProject.Data.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using FInalProject.Repositories.Interfaces;
using FInalProject.ViewModels.Author.AuthorOperations;
using FInalProject.ViewModels.Author.AuthorFiltering;
using FInalProject.ViewModels.Author;
using FInalProject.ViewModels.Book;
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
        private readonly UserManager<User> _userManager;
        private readonly IAuthorRepository _authorRepository;
        private readonly IFavouriteAuthorRepository _favouriteAuthorRepository;
        public AuthorsService(UserManager<User> userManager, IAuthorRepository authorRepository, IFavouriteAuthorRepository favouriteAuthorRepository)
        {
            _userManager = userManager;
            _authorRepository = authorRepository;
            _favouriteAuthorRepository = favouriteAuthorRepository;
        }

        public async Task<bool> AddPortraitToAuthorAsync(AddAuthorPortraitViewModel model)
        {
            var author = await _authorRepository.GetAuthorByIdAsync(model.Id);
            if (author == null) return false;
            author.Portrait = model.Picture;
            _authorRepository.UpdateAuthor(author);
            await _authorRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> FavouriteAuthorAsync(int authorId, ClaimsPrincipal User)
        {
            var currUser = await _userManager.GetUserAsync(User);
            if (currUser == null) return false;

            var favourite = await _favouriteAuthorRepository.GetFavouriteAuthorAsync(authorId, currUser.Id);
            var author = await _authorRepository.GetAuthorByIdAsync(authorId);
                if(favourite == null)
                {
                    FavouriteAuthor newFave = new FavouriteAuthor
                    {
                        User = currUser,
                        UserId = currUser.Id,
                        Author = author,
                        AuthorId = authorId
                    };
                await _favouriteAuthorRepository.AddFavouriteAuthorAsync(newFave);
                await _favouriteAuthorRepository.SaveChangesAsync();
                    return true;
                }
                _favouriteAuthorRepository.RemoveFavouriteAuthor(favourite);
            await _favouriteAuthorRepository.SaveChangesAsync();
               return true;
        }

        public async Task<List<AuthorListViewModel>> RenderAuthorListAsync()
        {
            return await _authorRepository.RenderAuthorListAsync();
        }
        public async Task<AuthorProfileViewModel> RenderAuthorProfileAsync(int authorId, ClaimsPrincipal User)
        {
            var empty = new AuthorProfileViewModel();
            var currUserId = _userManager.GetUserId(User);
            if (currUserId == null) return empty;

            var author = await _authorRepository.GetAuthorWithBooksByIdAsync(authorId);

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
                results.authorsFound = await _authorRepository.RenderAuthorSearchResutlsAsync(searchString);
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
