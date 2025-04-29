using FInalProject.Services;
using FInalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FInalProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        public AdminController (IAdminService adminService)
        {
            _adminService = adminService;
        }
       public async Task<IActionResult> AdminPanel(int setting)
        {
            
            AdminPanelViewModel model = new AdminPanelViewModel();
            model.BookOrUser = setting;
            model.UserList = await _adminService.RenderUserListAsync();
            model.BookList = await _adminService.RenderBookListAsync();
            return View(model); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminPanelSetting(int BookOrUser)
        {
            return RedirectToAction("AdminPanel", new {setting = BookOrUser});
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BanUser(string banId)
        {
            var succes = await _adminService.BanUser(banId);
            if(succes == true)
            {
                return Json(new {succes = true, redirectUrl = Url.Action("AdminPanel", "Admin") });
            }
            return Json(new { succes = false, message = "User not found" });
        }
    }
}
