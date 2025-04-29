using FInalProject.Data.Models;
using Microsoft.AspNetCore.Routing.Constraints;
using System.ComponentModel.DataAnnotations;

namespace FInalProject.ViewModels
{
    public class BookListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? AuthorName { get; set; }
        public double? Pages { get; set; }
        public string CoverImage { get; set; }
        public DateTime DateWritten { get; set; }
        public Category? Category { get; set; }
        public List<string>? Genres { get; set; } 


    }
}
