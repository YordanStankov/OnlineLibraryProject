using System.ComponentModel.DataAnnotations.Schema;

namespace FInalProject.Models
{
    public class Favourite
    {
        public int Amount { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }
        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
