
using FInalProject.Application.DTOs.Comment;
using FInalProject.Application.DTOs.Genre;
using FInalProject.Application.ViewModels.Comment;
using FInalProject.Application.ViewModels.Genre;

namespace FInalProject.Application.DTOs.Book
{
    public class BookFocusDTO
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
        public ICollection<GenreListDTO> genres { get; set; }
        public ICollection<CommentDTO>? comments { get; set; }
        public int Rating { get; set; }
    }
}
