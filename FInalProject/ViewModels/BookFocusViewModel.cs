using FInalProject.Models;
namespace FInalProject.ViewModels
{
    public class BookFocusViewModel
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public int BookPages { get; set; }
        public double BookReadingTime { get; set; }
        public string BookAuthorName { get; set; }
        public string BookCover { get; set; }
        public int AmountInStock { get; set; }
        public string Description { get; set; }

        public ICollection<Genre> genres { get; set; }
        public ICollection<CommentViewModel>? comments { get; set; }
    }
}
