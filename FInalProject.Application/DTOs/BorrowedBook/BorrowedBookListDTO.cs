
namespace FInalProject.Application.DTOs.BorrowedBook
{
    public class BorrowedBookListDTO
    {
        public int BookId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset? UntillReturn { get; set; }
        public string CoverImage { get; set; }
    }
}
