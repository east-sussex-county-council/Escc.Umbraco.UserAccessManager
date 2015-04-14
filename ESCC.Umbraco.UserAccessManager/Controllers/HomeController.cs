using System.Web.Mvc;
using ESCC.Umbraco.UserAccessManager.Utility;

namespace ESCC.Umbraco.UserAccessManager.Controllers
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