
using System.ComponentModel.DataAnnotations;

namespace FInalProject.Application.DTOs.Genre
{
    public class GenreEditDTO
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Genre name must be betwwen 3 and 50 characters")]
        public string Name { get; set; }
    }
}
