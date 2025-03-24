using FInalProject.Data.Models;

namespace FInalProject.ViewModels
{
    public class CreateCommentViewModel
    {
        public int BookId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        
    }
}
