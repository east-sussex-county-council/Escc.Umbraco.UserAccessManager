using Castle.Core.Internal;
using System.Web.Mvc;
using UmbracoUserControl.Models;
using UmbracoUserControl.Services.Interfaces;

namespace UmbracoUserControl.Controllers
{
    public class ToolsController : Controller
    {
        private readonly IPermissionsControlService permissionsControlService;

        public ToolsController(IPermissionsControlService permissionsControlService)
        {
            this.permissionsControlService = permissionsControlService;
        }

        // GET: Tools
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CheckPagePermissions(string url)
        {
            var modelList = permissionsControlService.CheckPagePermissions(url);

            if (!modelList.IsNullOrEmpty()) return PartialView("CheckPagePermissions", modelList);
            TempData["Message"] = "Either permissions have not been set for this page or page does not exist.";
            TempData["InputString"] = url;

            return PartialView("ToolsError");
        }

        [HttpGet]
        public ActionResult CheckUserPermissions(FindUserModel model)
        {
            var modelList = permissionsControlService.CheckUserPermissions(model);

            if (!modelList.IsNullOrEmpty()) return PartialView("CheckUserPermissions", modelList);

            TempData["Message"] = "Either user has no permissions setup or this user does not exist.";
            TempData["InputString"] = model.EmailAddress + model.UserName;

            return PartialView("ToolsError");
        }

        [HttpGet]
        public ActionResult CheckPageWithoutAuthor()
        {
            var modelList = permissionsControlService.PagesWithoutAuthor();

            if (!modelList.IsNullOrEmpty()) return PartialView("PagesWithoutAuthors", modelList);

            TempData["Message"] = "Unable to find pages without authors.";

            return PartialView("ToolsError");
        }
    }
}