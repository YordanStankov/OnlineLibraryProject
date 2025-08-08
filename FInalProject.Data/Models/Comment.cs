using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FInalProject.Domain.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(2000, MinimumLength = 3, ErrorMessage ="Comment must be between 3 and 2000 characters")]
        public string? CommentContent { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }

        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }
        public Book Book { get; set; }

        public ICollection<Favourite> favourites = new List<Favourite>();
    }
}
