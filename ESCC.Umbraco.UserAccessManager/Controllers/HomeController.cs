using System.Web.Mvc;
using Escc.Umbraco.UserAccessManager.Utility;

namespace Escc.Umbraco.UserAccessManager.Controllers
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