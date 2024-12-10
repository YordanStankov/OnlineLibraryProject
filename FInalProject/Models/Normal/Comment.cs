using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FInalProject.Models.Normal
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Range(0, 5)]
        public double Rating { get; set; }
        [Required]
        [MaxLength(300)]
        public string CommentContent { get; set; }
        [ForeignKey(nameof(User))]
        public string UserName { get; set; }
        public User User { get; set; }  
        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
