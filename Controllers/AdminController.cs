using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace MyApp.Namespace
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {

        public ActionResult Admin()
        {
            return View();
        }


        

    }
}
