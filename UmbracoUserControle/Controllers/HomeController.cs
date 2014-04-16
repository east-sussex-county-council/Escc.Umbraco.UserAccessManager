using System.Web.Mvc;

namespace UmbracoUserControl.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}