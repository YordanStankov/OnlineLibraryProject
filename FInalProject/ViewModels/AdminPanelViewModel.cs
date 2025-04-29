namespace FInalProject.ViewModels
{
    public class AdminPanelViewModel
    {
        public int BookOrUser { get; set; }
        public List<UserListViewModel> UserList { get; set; }
        public List<AdminBookListViewModel> BookList { get; set; }
    }
}
