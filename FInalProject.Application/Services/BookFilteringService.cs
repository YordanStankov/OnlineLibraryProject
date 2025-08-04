using FInalProject.Application.Interfaces;
using FInalProject.Application.ViewModels.Book;
using FInalProject.Application.ViewModels.Book.BookFiltering;

namespace FInalProject.Application.Services
{
    public interface IBookFilteringService
    {
        Task<SearchResultsViewModel> ReturnSearchResultsAync(string searchedString);
        Task<List<BookListViewModel>> ApplyFiltering(IEnumerable<BookListViewModel> books, FilteringViewModel filtering);

    }
    public class BookFilteringService : IBookFilteringService
    {
        private readonly IBookRepository _bookRepository;
        public BookFilteringService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<List<BookListViewModel>> ApplyFiltering(IEnumerable<BookListViewModel> books, FilteringViewModel filtering)
        {
            return await Task.FromResult(filtering.SortBy switch
            {
                "Name" => filtering.SortDirection == "desc"
                    ? books.OrderByDescending(b => b.Name).ToList()
                    : books.OrderBy(b => b.Name).ToList(),

                "Date" => filtering.SortDirection == "desc"
                    ? books.OrderByDescending(b => b.DateWritten).ToList()
                    : books.OrderBy(b => b.DateWritten).ToList(),

                _ => books.ToList()
            });
        }
        public async Task<SearchResultsViewModel> ReturnSearchResultsAync(string searchedString)
        {
            SearchResultsViewModel results = new SearchResultsViewModel
            {
                SearchQuery = searchedString
            };

            if (string.IsNullOrWhiteSpace(results.SearchQuery))
            {
                results.Message = "Nothing was typed into the search bar";
                return results;
            }

            var searchedBooks = await _bookRepository.RenderSearchedBookListAsync(results.SearchQuery);

            if (searchedBooks.Any())
            {
                results.Message = $"Search results for: {results.SearchQuery}";
                results.BooksMatchingQuery = searchedBooks;
                return results;
            }
            else
            {
                results.Message = $"No books found matching this search: {results.SearchQuery}";
            }
            return results;
        }
    }
}
