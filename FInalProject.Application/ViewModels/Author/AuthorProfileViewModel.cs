using FInalProject.Domain.Models;
using FInalProject.Application.ViewModels.Book;

namespace FInalProject.Application.ViewModels.Author
{
    public class AuthorProfileViewModel
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public bool isAuthorFavourited { get; set; }
        public string? AuthorPortrait { get; set; }
        public int FavouritesCount { get; set; }
        public ICollection<BookListViewModel> AuthorsBooks { get; set; }
    }
}
