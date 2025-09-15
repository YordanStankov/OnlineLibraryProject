
namespace FInalProject.Application.DTOs.Book
{
    public class BooksLeaderboardDTO
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string AuthorName { get; set; }
        public string CategoryString { get; set; }
        public int CommunityRating { get; set; } = 0;
    }
}
