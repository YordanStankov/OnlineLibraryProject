using Microsoft.AspNetCore.Mvc;

namespace FInalProject.Controllers
{
    public class UserErrorsController : Controller
    {
        public IActionResult LoginPlease()
        {
            return View();
        }
        public IActionResult EmptySearch()
        {
            return View();
        }
        public IActionResult NoneFromCategory()
        {
            return View();
        }
    }
}
