using MimeKit.Cryptography;

namespace FInalProject.ViewModels
{
    public class BooksFromGenreViewModel
    {
        public string Genre { get; set; }
        public string Message { get; set; }
        public ICollection<BookListViewModel>? BooksMatchingGenre { get; set; }
    }
}
