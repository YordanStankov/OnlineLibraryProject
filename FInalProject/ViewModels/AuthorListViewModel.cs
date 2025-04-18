using FInalProject.Data.Models;
using FInalProject.ViewModels;
namespace FInalProject.ViewModels
{
    public class AuthorListViewModel
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string? AuthorPortrait { get; set; }
        public int Favourites { get; set; }
    }
}
  