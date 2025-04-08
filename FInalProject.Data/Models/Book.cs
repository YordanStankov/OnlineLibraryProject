using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FInalProject.Data.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage ="Reading time must be between 1 and 1000 hours")]
        public double ReadingTime { get; set; }

        [Required]
        [Range(1, 10000, ErrorMessage ="Pages must be between 1 and 10000 pages")]
        public int Pages { get; set; }

        [ForeignKey(nameof(Author))]
        [Required]
        public int AuthorId { get; set; }
        [Required]
        public Author Author { get; set; }

        [Required]
        public Category Category { get; set; }
        public string CategoryString { get; set; }
        
        [Required]
        public string CoverImage { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 5, ErrorMessage ="Description must be between 5 and 1000 words")]
        public string Description { get; set; }

        [Required]
        public int AmountInStock { get; set; }

        public ICollection<Comment>? Comments { get; set; } 
        public ICollection<BookGenre>? BookGenres { get; set; } 
        public ICollection<Favourite>? Favourites { get; set; }
        public ICollection<BorrowedBook> BorrowedBooks { get; set; } 
    }
}
