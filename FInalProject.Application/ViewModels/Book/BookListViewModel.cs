
namespace FInalProject.Application.ViewModels.Book
{
    public class BookListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? AuthorName { get; set; }
        public double? Pages { get; set; }
        public string CoverImage { get; set; }
        public DateTime DateWritten { get; set; }
        public string? Category { get; set; }
        public List<string>? Genres { get; set; } 


    }
}
