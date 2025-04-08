using FInalProject.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace FInalProject.ViewModels
{
    public class CreateCommentViewModel
    {
        public int BookId { get; set; }
        public int UserId { get; set; }
        [Required]
        [StringLength(2000, MinimumLength = 3, ErrorMessage = "Comment must be between 3 and 2000 characters")]
        public string Description { get; set; }
        
    }
}
