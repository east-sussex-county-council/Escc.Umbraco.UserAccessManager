using System.Linq;
using System.Web.Mvc;
using ESCC.Umbraco.UserAccessManager.Models;
using ESCC.Umbraco.UserAccessManager.Services.Interfaces;

namespace ESCC.Umbraco.UserAccessManager.Controllers
{
    public class PageAuthorController : Controller
    {
        private readonly IPermissionsControlService _permissionsControlService;

        public PageAuthorController(IPermissionsControlService permissionsControlService)
        {
            _permissionsControlService = permissionsControlService;
        }

        // GET: PageAuthor
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CheckPagePermissions(string url)
        {
            var modelList = _permissionsControlService.CheckPagePermissions(url);

            // If nodelList is null then the page does not exist in Umbraco
            if (modelList == null)
            {
                TempData["Message"] = "Sorry, that page does not exist.";
                TempData["InputString"] = url;

                return PartialView("Error");
            }

            // if the permissions list has no entries then no specific permissions have been set
            if (!modelList.Any())
            {
                TempData["Message"] = "Permissions have not been set for this page.";
                TempData["InputString"] = url;

                return PartialView("Error");
            }

            // One or more Web Authors have been given permission to edit this page
            return PartialView("CheckPagePermissions", modelList);
        }
    }
}