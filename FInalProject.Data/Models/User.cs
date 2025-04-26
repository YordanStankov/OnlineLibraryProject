using Microsoft.AspNetCore.Identity;

namespace FInalProject.Data.Models
{
    public class User : IdentityUser
    {
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();
        public ICollection<BorrowedBook> BorrowedBooks { get; set; } = new List<BorrowedBook>();
        public ICollection<FavouriteAuthor> FavouriteAuthors { get; set; } = new List<FavouriteAuthor>();
        public int? Strikes { get; set; }
        public bool CanBorrow { get; set; }
    }
}
