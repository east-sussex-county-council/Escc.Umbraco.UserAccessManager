using System.Web.Mvc;
using UmbracoUserControl.Utility;

namespace UmbracoUserControl.Controllers
{
    [AuthorizeRedirect(Roles = SystemRole.AllAuthorised)]
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Unauthorized()
        {
            return View();
        }
    }
}