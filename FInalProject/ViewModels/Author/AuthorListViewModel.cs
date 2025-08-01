using FInalProject.Data.Models;
namespace FInalProject.ViewModels.Author
{
    public class AuthorListViewModel
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string? AuthorPortrait { get; set; }
        public int Favourites { get; set; }
    }
}
  