using FInalProject.Domain.Models;
using FInalProject.Application.Interfaces;
using FInalProject.Application.ViewModels.Book.BookFiltering;
using FInalProject.Application.ViewModels.Genre.GenreOprations;
using Microsoft.Extensions.Logging;

namespace FInalProject.Application.Services
{
    public interface IGenreService 
    {
        Task<bool> DeleteGenreAsync(int doomedGenreId);
        Task<bool> AddGenreAsync(string Name);
        Task<BooksFromGenreViewModel> GetAllBooksOfCertainGenre(int genreId);
        Task<List<Genre>> GetGenreListAsync();
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
            var existingGenre = await _genreRepository.GetGenreByNameAsync(Name);
            if (existingGenre == null)
            {
                _logger.LogInformation("ADDING NEW GENRE");
                Genre genre = new Genre
                {
                    Name = Name
                };
                await _genreRepository.AddGenreAsync(genre);
                await _genreRepository.SaveChangesAsync();
                _logger.LogInformation("ADDED THE GENRE");
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteGenreAsync(int doomedGenreId)
        {
            _logger.LogInformation("DELETE GENRE METHOD");
            var doomedGenre = await _genreRepository.GetGenreByIdAsync(doomedGenreId);
            if (doomedGenre != null)
            {
                _genreRepository.RemoveGenre(doomedGenre);
                await _genreRepository.SaveChangesAsync();
                _logger.LogInformation("REMOVED THE GENRE");
                return true;
            }
            return false;
        }

        //Providing lists of genres
        public async Task<List<Genre>> GetGenreListAsync()
        {
            _logger.LogInformation("GETING ALL GENRES METHOD");
            var genreList = await _genreRepository.GetAllGenresAsync();
            return genreList;
        }

        public async Task<BooksFromGenreViewModel> GetAllBooksOfCertainGenre(int genreId)
        {
            BooksFromGenreViewModel GenreBooks = new BooksFromGenreViewModel();

            var genre = await _genreRepository.GetGenreByIdAsync(genreId);
            GenreBooks.Genre = genre?.Name ?? "Null";

            GenreBooks.BooksMatchingGenre = await _genreRepository.RenderSpecificGenreBookListAsync(genreId);

            if (GenreBooks.BooksMatchingGenre == null || !GenreBooks.BooksMatchingGenre.Any())
            {
                GenreBooks.Message = $"No books from genre {GenreBooks.Genre}";
                return GenreBooks;
            }
            else
            {
                GenreBooks.Message = $"Books found from {GenreBooks.Genre}";
                return GenreBooks;
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
            var genreNeedsEdit = await _genreRepository.GetGenreByIdAsync(model.Id);
            if(genreNeedsEdit != null)
            {
                genreNeedsEdit.Name = model.Name;
                _logger.LogInformation("CHANGED THE NAME OF THE GENRE WOOOOOOOOOOOO");
                await _genreRepository.SaveChangesAsync();
                return true;
            }
            _logger.LogError("COULDNT FIND GENRE WHEN SAVING THE CHANGES TO IT");
            return false;
        }
    }
}
