using FInalProject.Data.Models;
using FInalProject.ViewModels.Comment;
namespace FInalProject.ViewModels.Book
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
        public Category Category { get; set; }

        public bool Borrowed { get; set; }
        public ICollection<FInalProject.Data.Models.Genre> genres { get; set; }
        public ICollection<CommentViewModel>? comments { get; set; }
        public ICollection<Favourite>? Favourites { get; set; }

    }
}
