using FInalProject.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace FInalProject.ViewModels.Comment
{
    public class CommentViewModel
    {
        public string UserName { get; set; }
       
        public string Description { get; set; }
    }
}
