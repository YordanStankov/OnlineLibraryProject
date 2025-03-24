using System.ComponentModel.DataAnnotations;

namespace FInalProject.Data.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]  
        public string Name { get; set; }
        public ICollection<BookGenre>? BookGenres { get; set; } = new List<BookGenre>(); 
    }
}
