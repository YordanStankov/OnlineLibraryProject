using FInalProject.Data.Models;
using FInalProject.ViewModels;
namespace FInalProject.ViewModels
{
    public class AuthorProfileViewModel
    {
        public string AuthorName { get; set; }
        public string? AuthorPortrait { get; set; }
        public ICollection<FavouriteAuthor>? FavouriteAuthors { get; set; }
        public int FavouritesCount { get; set; }
        public ICollection<BookListViewModel> AuthorsBooks { get; set; }
    }
}
