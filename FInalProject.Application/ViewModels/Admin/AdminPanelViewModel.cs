using FInalProject.Application.ViewModels.Admin.Book;
using FInalProject.Application.ViewModels.Admin.User;

namespace FInalProject.Application.ViewModels.Admin
{
    public class AdminPanelViewModel
    {
        public int BookOrUser { get; set; }
        public List<UserListViewModel> UserList { get; set; }
        public List<AdminBookListViewModel> BookList { get; set; }
    }
}
