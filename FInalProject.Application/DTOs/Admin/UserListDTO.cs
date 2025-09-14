
namespace FInalProject.Application.DTOs.Admin
{
    public class UserListDTO
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public int Strikes { get; set; }
        public bool CantBorrow { get; set; }
    }
}
