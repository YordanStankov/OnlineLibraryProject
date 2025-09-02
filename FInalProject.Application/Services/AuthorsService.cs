using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using FInalProject.Application.Interfaces;
using FInalProject.Application.ViewModels.Author.AuthorOperations;
using FInalProject.Application.ViewModels.Author.AuthorFiltering;
using FInalProject.Application.ViewModels.Author;
using FInalProject.Application.DTOs.AuthorDTOs;
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
        private readonly IAuthorRepository _authorRepository;
        private readonly IFavouriteAuthorRepository _favouriteAuthorRepository;
        public AuthorsService(IAuthorRepository authorRepository, IFavouriteAuthorRepository favouriteAuthorRepository)
        {
            _authorRepository = authorRepository;
            _favouriteAuthorRepository = favouriteAuthorRepository;
        }

        public async Task<bool> AddPortraitToAuthorAsync(AddAuthorPortraitViewModel model)
        {
            AddAuthorPortraitDTO dto = new AddAuthorPortraitDTO(model.Picture, model.Id);
            if (dto == null) return false;
            await _authorRepository.AddPortraitToAuthorAsync(dto);
            return true;
        }

        public async Task<bool> FavouriteAuthorAsync(int authorId, ClaimsPrincipal User)
        {
            if (User == null) return false;
            FavouriteAuthorDTO dto = new FavouriteAuthorDTO(User, authorId);
            var result = await _favouriteAuthorRepository.AddNewFavouriteAuthorAsync(dto);
            return result;  
        }

        public async Task<List<AuthorListViewModel>> RenderAuthorListAsync()
        {
            return await _authorRepository.RenderAuthorListAsync();
        }
        public async Task<AuthorProfileViewModel> RenderAuthorProfileAsync(int authorId, ClaimsPrincipal User)
        {
          var model = await _authorRepository.RenderAuthorProfileASync(new FavouriteAuthorDTO(User, authorId));
            return model;
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
