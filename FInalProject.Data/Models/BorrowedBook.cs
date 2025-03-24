using System.ComponentModel.DataAnnotations.Schema;

namespace FInalProject.Data.Models
{
    public class BorrowedBook
    {
        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }
        public Book Book { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
