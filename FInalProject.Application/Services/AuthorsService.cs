using FInalProject.Domain.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using FInalProject.Application.Interfaces;
using FInalProject.Application.ViewModels.Author.AuthorOperations;
using FInalProject.Application.ViewModels.Author.AuthorFiltering;
using FInalProject.Application.ViewModels.Author;
using FInalProject.Application.ViewModels.Book;
namespace FInalProject.Application.Services
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
                        UserId = currUser.Id,
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
            var authors = await _authorRepository.ReturnAuthorListAsync();
            return authors.Select(a => new AuthorListViewModel
            {
                AuthorId = a.Id,
                AuthorPortrait = a.Portrait,
                AuthorName = a.Name,
                Favourites = a.FavouriteAuthors.Count()
            }).ToList();
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
                    Category = n.Category.ToString(),
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
            var authors = await _authorRepository.ReturnSearchedAuthorListAsync(searchString);
           
            results.authorsFound = authors.Select(a => new AuthorListViewModel
            {
                AuthorId = a.Id,
                AuthorName = a.Name,
                AuthorPortrait = a.Portrait,
                Favourites = a.FavouriteAuthors?.Count() ?? 0
            }).ToList();
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
