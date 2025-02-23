using FInalProject.Models;
using System.ComponentModel.DataAnnotations;
namespace FInalProject.ViewModels
{
    public class BookCreationViewModel
    {
        public string Name { get; set; }
        public int Pages { get; set; }
        public Author Author { get; set; }
        public double ReadingTime { get; set; }
        public string CoverImage { get; set; }
        public string Description { get; set; }

        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    }
}
