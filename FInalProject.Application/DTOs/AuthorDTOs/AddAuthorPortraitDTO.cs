
namespace FInalProject.Application.DTOs.AuthorDTOs
{
    public class AddAuthorPortraitDTO : AuthorDTOBase
    {
        public AddAuthorPortraitDTO(string Portrait, int id): base(id)
        {
            this.Portrait = Portrait;
        }
        public string Portrait { get; set; }    

    }
}
