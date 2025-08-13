
namespace FInalProject.Application.ViewModels.User.UserOperations
{
    public class ChangePasswordOperationViewModel
    {
        public string UserId { get; set; }
        
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
