using FInalProject.ViewModels.Admin.Book;
using FInalProject.ViewModels.Admin.User;

namespace FInalProject.ViewModels.Admin
{
    public class AdminPanelViewModel
    {
        public int BookOrUser { get; set; }
        public List<UserListViewModel> UserList { get; set; }
        public List<AdminBookListViewModel> BookList { get; set; }
    }
}
