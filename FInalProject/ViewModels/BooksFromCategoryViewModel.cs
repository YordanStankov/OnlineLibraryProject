
namespace FInalProject.ViewModels
{
    public class BooksFromCategoryViewModel
    {
        public string Category { get; set; }
        public ICollection<BookListViewModel>? BooksFromCategory { get; set; }
        public string Message { get; set; }
    }
}
