using FInalProject.Domain.Models;
using FInalProject.Application.Interfaces;
using FInalProject.Application.ViewModels.Book.BookFiltering;
using FInalProject.Application.ViewModels.Genre.GenreOprations;
using Microsoft.Extensions.Logging;
using FInalProject.Application.ViewModels.Genre;
using FInalProject.Application.ViewModels.Book;

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
        public async Task<List<GenreListViewModel>> GetGenreListAsync()
        {
            _logger.LogInformation("GETING ALL GENRES METHOD");
            var genreList = await _genreRepository.GetAllGenresDTOAsync();
            var ViewModels = genreList.Select(g => new GenreListViewModel
            {
                Id = g.Id,
                Name = g.Name
            }).ToList();
            return ViewModels;
        }

        public async Task<BooksFromGenreViewModel> GetAllBooksOfCertainGenre(int genreId)
        {
            BooksFromGenreViewModel GenreBooks = new BooksFromGenreViewModel();

            var genre = await _genreRepository.GetGenreByIdAsync(genreId);
            GenreBooks.Genre = genre?.Name ?? "Null";

            var books = await _genreRepository.GetSpecificGenreBookListDTOAsync(genreId);
            GenreBooks.BooksMatchingGenre = books.Select(b => new BookListViewModel
            {
                Id = b.Id,
                Name = b.Name,
                DateWritten = b.DateWritten,
                CoverImage = b.CoverImage,
                AuthorName = b.AuthorName,
                Category = b.Category,
                Genres = b.Genres
            }).ToList();

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
            var viewModel = await _genreRepository.ReturnSingleGenreDTOToEditAsync(genreEditId);
            if (viewModel == null)
                throw new Exception("GenreEditViewModel is null. Check GenreService");
            var final = new GenreEditViewModel
            {
                Id = viewModel.Id,
                Name = viewModel.Name
            };
            return final;
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
