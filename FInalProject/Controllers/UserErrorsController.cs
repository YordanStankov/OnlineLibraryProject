using Microsoft.AspNetCore.Mvc;

namespace FInalProject.Controllers
{
    public class UserErrorsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LoginPlease()
        {
            return View();
        }
    }
}
