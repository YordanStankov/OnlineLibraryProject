
namespace FInalProject.Application.DTOs.AuthorDTOs
{
    public class AuthorDTOBase
    {
        public AuthorDTOBase(int id)
        {
            Id = id;
        }
        public int Id { get; set; }
    }
}
