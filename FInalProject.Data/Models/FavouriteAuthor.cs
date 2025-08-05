using System.ComponentModel.DataAnnotations.Schema;

namespace FInalProject.Data.Models
{
   public class FavouriteAuthor
    {
        [ForeignKey(nameof(Author))]
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User? User { get; set; }
     }
}
