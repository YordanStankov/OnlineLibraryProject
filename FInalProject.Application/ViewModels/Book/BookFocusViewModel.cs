using FInalProject.Application.ViewModels.Comment;
using FInalProject.Application.ViewModels.Genre;
namespace FInalProject.Application.ViewModels.Book
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
        public DateTime DateWritten { get; set; }
        public string Category { get; set; }
        public bool Borrowed { get; set; }
        public ICollection<GenreListViewModel> genres { get; set; }
        public ICollection<CommentViewModel>? comments { get; set; }
        public int Rating { get; set; } = 0;

        }
}
