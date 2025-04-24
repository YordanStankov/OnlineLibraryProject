namespace FInalProject.ViewModels
{
    public class AuthorSearchResultsViewModel
    {
        public ICollection<AuthorListViewModel>? authorsFound { get; set; }
        public string SearchQuery { get; set; }
        public string Message { get; set; }
    }
}
