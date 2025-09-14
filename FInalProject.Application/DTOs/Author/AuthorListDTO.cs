
namespace FInalProject.Application.DTOs.Author
{
    public class AuthorListDTO
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string? AuthorPortrait { get; set; }
        public int Favourites { get; set; } = 0;
    }
}
