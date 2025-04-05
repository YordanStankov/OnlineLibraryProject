using FInalProject.Data.Models;
using System.ComponentModel.DataAnnotations;
namespace FInalProject.ViewModels
{
    public class BookCreationViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Pages { get; set; }
        public string AuthorName { get; set; }
        public double ReadingTime { get; set; }
        public string CoverImage { get; set; }
        public string Description { get; set; }
        public int AmountInStock { get; set; }
        public Category Category { get; set; }
        public int? editor { get; set; }
        public ICollection<Genre>? GenreOptions { get; set; } 
        public ICollection<int> SelectedGenreIds { get; set; } 
    }
}
