using FInalProject.Domain.Models;
using FInalProject.Application.Interfaces;
using FInalProject.Application.ViewModels.Book;


namespace FInalProject.Application.Services
{
    public interface IBookCRUDService
    {
        Task<bool> EditBookAsync(BookCreationViewModel model);
        Task<bool> DeleteBookAsync(int doomedId);
        Task<int> CreateBookAsync(BookCreationViewModel model);
    }
    public class BookCRUDService : IBookCRUDService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookGenreRepository _bookGenreRepository;
        private readonly IAuthorRepository _authorRepository;

        public BookCRUDService(IBookRepository bookRepository, IBookGenreRepository bookGenreRepository, IAuthorRepository authorRepository) 
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _bookGenreRepository = bookGenreRepository;
        }
        public async Task<bool> EditBookAsync(BookCreationViewModel model)
        {
            
            var bookToEdit = await _bookRepository.ReturnBookEntityToEditAsync(model.Id);

            if (model.editor == 0)
            {
                MapAdminFields(bookToEdit, model);

                await MapAuthor(bookToEdit, model);

                await MapBookGenres(bookToEdit, model);

                _authorRepository.UpdateAuthor(bookToEdit.Author);
                _bookRepository.UpdateBook(bookToEdit);
                await _bookRepository.SaveChangesAsync();
                return true;
            }
            else if (model.editor == 1)
            {
                
                bookToEdit.Name = model.Name;
                _bookRepository.UpdateBook(bookToEdit);
                await _bookRepository.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteBookAsync(int doomedId)
        {
            var doomedBook = await _bookRepository.ReturnBookEntityToDeleteAsync(doomedId);

            if (doomedBook != null)
            {
                _bookRepository.RemoveBook(doomedBook);
                await _bookRepository.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<int> CreateBookAsync(BookCreationViewModel model)
        {
            var existingAuthor = await _authorRepository.GetAuthorByNameAsync(model.AuthorName);
            var correctAuthor = existingAuthor ?? new Author { Name = model.AuthorName };
            var newBook = new Book();
            MapBook(newBook, model, correctAuthor);

            _bookRepository.AddBook(newBook);

            if (model.SelectedGenreIds != null)
            {
                List<BookGenre> newBookGenres = new List<BookGenre>();
                foreach (var genreId in model.SelectedGenreIds)
                {
                    var bookGenre = new BookGenre
                    {
                        BookId = newBook.Id,
                        GenreId = genreId
                    };
                    newBookGenres.Add(bookGenre);
                }
                await _bookGenreRepository.AddListOfNewBookGenresAsync(newBookGenres);
            }
            _authorRepository.AddToAuhtorBookList(correctAuthor, newBook);
            await _bookRepository.SaveChangesAsync();
            return newBook.Id;
        }




        //MAPPING METHODS
        private void MapBook(Book book, BookCreationViewModel model, Author correctAuthor)
        {
            book.Name = model.Name;
            book.ReadingTime = model.ReadingTime;
            book.Pages = model.Pages;
            book.Author = correctAuthor;
            book.DateWritten = model.DateWritten;
            book.AmountInStock = model.AmountInStock;
            book.Category = model.Category;
            book.CategoryString = model.Category.ToString();
            book.CoverImage = model.CoverImage;
            book.Description = model.Description;
        }
        private void MapAdminFields(Book bookToEdit, BookCreationViewModel model)
        {
            bookToEdit.Name = model.Name;
            bookToEdit.DateWritten = model.DateWritten;
            bookToEdit.AmountInStock = model.AmountInStock;
            bookToEdit.Pages = model.Pages;
            bookToEdit.Category = model.Category;
            bookToEdit.CategoryString = model.Category.ToString();
            bookToEdit.Description = model.Description;
            bookToEdit.CoverImage = model.CoverImage;
            bookToEdit.ReadingTime = model.ReadingTime;
        }
        private async Task MapAuthor(Book bookToEdit, BookCreationViewModel model)
        {
            var searchedAuthor = await _authorRepository.GetAuthorByIdAsync(bookToEdit.AuthorId);
            searchedAuthor.Name = model.AuthorName;
        }
        private async Task MapBookGenres(Book bookToEdit, BookCreationViewModel model)
        {
            var existingGenreIds = bookToEdit.BookGenres.Select(bg => bg.GenreId).ToList();
            if (existingGenreIds != null && model.SelectedGenreIds != null)
            {
                var newGenres = model.SelectedGenreIds.Except(existingGenreIds);
                var removedGenres = existingGenreIds.Except(model.SelectedGenreIds);
                bookToEdit.BookGenres = bookToEdit.BookGenres.Where(bg => !removedGenres.Contains(bg.GenreId)).ToList();

                List<BookGenre> bookGenresToAdd = new List<BookGenre>();
                foreach (var genreId in newGenres)
                {
                    bookGenresToAdd.Add(new BookGenre { BookId = bookToEdit.Id, GenreId = genreId });
                }
                if (bookGenresToAdd.Count > 1)
                    await _bookGenreRepository.AddListOfNewBookGenresAsync(bookGenresToAdd);
                else if (bookGenresToAdd.Count == 1)
                    await _bookGenreRepository.AddNewBookGenreAsync(bookGenresToAdd[0]);
            }

        }
    }
}
