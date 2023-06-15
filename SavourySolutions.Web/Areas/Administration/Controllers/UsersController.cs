using Microsoft.AspNetCore.Mvc;

namespace SavourySolutions.Web.Areas.Administration.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
