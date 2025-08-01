namespace FInalProject.ViewModels.Admin.User
{
    public class UserListViewModel
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public int Strikes { get; set; }
        public bool CantBorrow { get; set; }

    }
}
