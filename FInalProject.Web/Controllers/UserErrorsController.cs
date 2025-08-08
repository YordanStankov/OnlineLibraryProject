using Microsoft.AspNetCore.Mvc;

namespace FInalProject.Web.Controllers
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
