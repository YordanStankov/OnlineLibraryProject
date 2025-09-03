using FInalProject.Application.Interfaces;
using FInalProject.Application.ViewModels.Book.BookFiltering;
using FInalProject.Application.ViewModels.Genre.GenreOprations;
using Microsoft.Extensions.Logging;
using FInalProject.Application.ViewModels.Genre;

namespace FInalProject.Application.Services
{
    public interface IGenreService 
    {
        Task<bool> DeleteGenreAsync(int doomedGenreId);
        Task<bool> AddGenreAsync(string Name);
        Task<BooksFromGenreViewModel> GetAllBooksOfCertainGenre(int genreId);
        Task<List<GenreListViewModel>> GetGenreListAsync();
        Task<GenreEditViewModel> ProvideGenreForPartialAsync(int genreEditId);
        Task<bool> SaveChangesToGenreAsync(GenreEditViewModel model);
    }


    public class GenreService : IGenreService
    {
        private readonly ILogger<GenreService> _logger;
   
        private readonly IGenreRepository _genreRepository;

        public GenreService(ILogger<GenreService> logger,IGenreRepository genreRepository)
        {
            
            _logger = logger;
            _genreRepository = genreRepository;
        }

        //Adding and deleting genres
        public async Task<bool> AddGenreAsync(string Name)
        {
            _logger.LogInformation("ADDING GENRE METHOD");
            var result = await _genreRepository.AddGenreAsync(Name);
            if (result == false)
            {
                _logger.LogError("GENRE ALREADY EXISTS");
                return false;
            }
            _logger.LogInformation("ADDED THE GENRE");
            return true;
        }

        public async Task<bool> DeleteGenreAsync(int doomedGenreId)
        {
            _logger.LogInformation("DELETE GENRE METHOD");
           var result = await _genreRepository.DeleteGenreAsync(doomedGenreId);
            if (result == false)
            {
                _logger.LogError("COULDNT FIND GENRE TO DELETE");
                return false;
            }
            _logger.LogInformation("DELETED THE GENRE");
            return true;
        }

        //Providing lists of genres
        public async Task<List<GenreListViewModel>> GetGenreListAsync()
        {
            _logger.LogInformation("GETING ALL GENRES METHOD");
            var genreList = await _genreRepository.GetAllGenresAsync();
            return genreList;
        }

        public async Task<BooksFromGenreViewModel> GetAllBooksOfCertainGenre(int genreId)
        {
            var genreBooks = await _genreRepository.GetAllBooksOfCertainGenreAsync(genreId);
            if (genreBooks.BooksMatchingGenre == null || !genreBooks.BooksMatchingGenre.Any())
            {
                genreBooks.Message = $"No books from genre {genreBooks.Genre}";
                return genreBooks;
            }
            else
            {
                genreBooks.Message = $"Books found from {genreBooks.Genre}";
                return genreBooks;
            }
        }

        //Genre editing
        public async Task<GenreEditViewModel> ProvideGenreForPartialAsync(int genreEditId)
        {
            var viewModel = await _genreRepository.ReturnSingleGenreToEditAsync(genreEditId);
            if (viewModel == null)
                throw new Exception("GenreEditViewModel is null. Check GenreService");
            return viewModel;
        }

        public async Task<bool> SaveChangesToGenreAsync(GenreEditViewModel model)
        {
          var result = await _genreRepository.SaveChangesToGenreAsync(model);
            if (result == false)
            {
                _logger.LogError("COULDNT SAVE CHANGES TO GENRE");
                return false;
            }
            _logger.LogInformation("SAVED CHANGES TO GENRE");
            return true;
        }
    }
}
