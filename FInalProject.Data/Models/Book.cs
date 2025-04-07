using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FInalProject.Data.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double ReadingTime { get; set; }

        [Required]
        public int Pages { get; set; }

        [ForeignKey(nameof(Author))]
        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public DateTime DateTaken { get; set; }
        public DateTimeOffset UntillReturn { get; set; }
        public Category Category { get; set; }
        public string? CategoryString { get; set; }
        
        [Required]
        public string CoverImage { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; }

        [Required]
        public int AmountInStock { get; set; }

        public ICollection<Comment>? Comments { get; set; } 
        public ICollection<BookGenre>? BookGenres { get; set; } 
        public ICollection<Favourite>? Favourites { get; set; }
        public ICollection<BorrowedBook> BorrowedBooks { get; set; } 
    }
}
