namespace FInalProject.ViewModels
{
    public class SearchResultsViewModel
    {
        public ICollection<BookListViewModel>? BooksMatchingQuery { get; set; }
        public string? Message { get; set; }
        public string? SearchQuery { get; set; }
    }
}
