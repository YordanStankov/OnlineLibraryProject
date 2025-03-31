using FInalProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace FInalProject.Controllers
{
    public class UserOperationsController : Controller
    {
        private readonly IUserOperationsService _userOperationsService;
        public UserOperationsController(IUserOperationsService userOperationsService)
        {
            _userOperationsService = userOperationsService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
