
namespace FInalProject.Application.ViewModels.Book
{
    public class BorrowedBookListViewModel
    {
        public int BookId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset? UntillReturn { get; set; }
        public string CoverImage { get; set; }
    }
}
