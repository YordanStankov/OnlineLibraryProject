using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
