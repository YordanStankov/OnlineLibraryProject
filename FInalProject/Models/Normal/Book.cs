using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FInalProject.Models.Normal
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [MaxLength(50)]
        public string Genre { get; set; }
        [Required]
        public int Pages { get; set; }
        [Required]
        public double ReadingTime { get; set; }
        [Required]
        public string CoverImage { get; set; }
        public double? Rating { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Download> Downloads { get; set;} = new List<Download>();
    }
}
