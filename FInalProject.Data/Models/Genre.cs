using System.ComponentModel.DataAnnotations;

namespace FInalProject.Domain.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage ="Genre name must be betwwen 3 and 50 characters")]  
        public string Name { get; set; }
        public ICollection<BookGenre>? BookGenres { get; set; } = new List<BookGenre>(); 
    }
}
