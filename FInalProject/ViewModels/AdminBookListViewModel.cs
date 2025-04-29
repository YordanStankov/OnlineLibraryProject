namespace FInalProject.ViewModels
{
    public class AdminBookListViewModel
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public int BookStock { get; set; }
        public List<string> genres { get; set; }
        public string Category { get; set; }
        public int BooksBorrowed { get; set; }
    }
}
