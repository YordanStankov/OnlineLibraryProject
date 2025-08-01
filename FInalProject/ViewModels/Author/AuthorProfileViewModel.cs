using FInalProject.Data.Models;
using FInalProject.ViewModels.Book;

namespace FInalProject.ViewModels.Author
{
    public class AuthorProfileViewModel
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public bool isAuthorFavourited { get; set; }
        public string? AuthorPortrait { get; set; }
        public ICollection<FavouriteAuthor>? FavouriteAuthors { get; set; }
        public int FavouritesCount { get; set; }
        public ICollection<BookListViewModel> AuthorsBooks { get; set; }
    }
}
