using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FInalProject.Models
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
        
        [Required]
        public string CoverImage { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; }

        [Required]
        public int AmountInStock { get; set; }

        public ICollection<Comment>? Comments { get; set; } = new List<Comment>();
        public ICollection<BookGenre>? BookGenres { get; set; } = new List<BookGenre>();
        public ICollection<Favourite>? Favourites { get; set; } = new List<Favourite>();
        public ICollection<BorrowedBook> BorrowedBooks { get; set; } = new List<BorrowedBook>();
    }
}
