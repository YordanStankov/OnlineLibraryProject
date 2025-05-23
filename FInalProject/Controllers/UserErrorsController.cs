using Microsoft.AspNetCore.Mvc;

namespace FInalProject.Controllers
{
    [Route("UserErrors/NotFound404")]
    public class UserErrorsController : Controller
    {
        [HttpGet]
        public IActionResult Error404()
        {
            return View();
        }
    }
}
