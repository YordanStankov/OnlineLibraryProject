using System.ComponentModel.DataAnnotations;

namespace FInalProject.Data.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage ="Author name must be between 3 and 50 characters")]
        public string Name { get; set; }
        public ICollection<Book>? Books { get; set; } 
    }
}
