
namespace FInalProject.Application.DTOs.UserDTOs
{
    public class UserDTOBase
    {
        public UserDTOBase(string id, string userName)
        {
            Id = id;
            UserName = userName;
        }
        public string Id { get; set; }
        public string UserName { get; set; }    
    }
}
