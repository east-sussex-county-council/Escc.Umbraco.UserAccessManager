using Castle.Core.Internal;
using System.Web.Mvc;
using UmbracoUserControl.Models;
using UmbracoUserControl.Services.Interfaces;

namespace UmbracoUserControl.Controllers
{
    public class ToolsController : Controller
    {
        private readonly IPermissionsControlService permissionsControlService;
        private readonly IUserControlService userControlService;

        public ToolsController(IPermissionsControlService permissionsControlService, IUserControlService userControlService)
        {
            this.permissionsControlService = permissionsControlService;
            this.userControlService = userControlService;
        }

        // GET: Tools
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CheckPagePermissions(string url)
        {
            var modelList = permissionsControlService.CheckPagePermissions(url);

            if (!modelList.IsNullOrEmpty()) return PartialView("CheckPagePermissions", modelList);
            TempData["Message"] = "Either permissions have not been set for this page or page does not exist.";
            TempData["InputString"] = url;

            return PartialView("ToolsError");
        }

        public ActionResult CheckUserPermissions(FindUserModel model)
        {
            var modelList = permissionsControlService.CheckUserPermissions(model);

            if (!modelList.IsNullOrEmpty()) return PartialView("CheckUserPermissions", modelList);

            TempData["Message"] = "Either user has no permissions setup or this user does not exist.";
            TempData["InputString"] = model.EmailAddress + model.UserName;

            return PartialView("ToolsError");
        }

        public ActionResult CheckPageWithoutAuthor()
        {
            var modelList = permissionsControlService.PagesWithoutAuthor();

            if (!modelList.IsNullOrEmpty()) return PartialView("PagesWithoutAuthors", modelList);

            TempData["Message"] = "Unable to find pages without authors.";

            return PartialView("ToolsError");
        }
    }
}