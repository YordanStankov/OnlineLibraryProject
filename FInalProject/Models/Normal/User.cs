using Microsoft.AspNetCore.Identity;

namespace FInalProject.Models.Normal
{
    public class User:IdentityUser
    {
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Download> Downloads { get; set;} = new List<Download>();
    }
}
