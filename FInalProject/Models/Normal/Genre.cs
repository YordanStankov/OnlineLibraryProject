using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FInalProject.Models.Normal
{
    public class Genre
    {
        [Key]
        public int Id { get; set; } 
        public string Name { get; set; }
        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
