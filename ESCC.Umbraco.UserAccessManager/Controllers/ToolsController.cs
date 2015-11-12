using System.Web.Mvc;
using Castle.Core.Internal;
using ESCC.Umbraco.UserAccessManager.Models;
using ESCC.Umbraco.UserAccessManager.Services.Interfaces;
using ESCC.Umbraco.UserAccessManager.Utility;

namespace ESCC.Umbraco.UserAccessManager.Controllers
{
    [AuthorizeRedirect(Roles = SystemRole.WebServices)]
    public class ToolsController : Controller
    {
        private readonly IPermissionsControlService _permissionsControlService;
        private readonly IUmbracoService _umbracoService;

        public ToolsController(IPermissionsControlService permissionsControlService, IUmbracoService umbracoService)
        {
            _permissionsControlService = permissionsControlService;
            _umbracoService = umbracoService;
        }

        // GET: Tools
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LookupPermissions()
        {
            return View("LookupPermissions/Index");
        }

        [HttpGet]
        public ActionResult PagePermissions()
        {
            return View("PagePermissions/Index");
        }

        [HttpGet]
        public ActionResult CheckPagePermissions(string url)
        {
            var modelList = _permissionsControlService.CheckPagePermissions(url);

            if (!modelList.IsNullOrEmpty()) return PartialView("CheckPagePermissions", modelList);

            TempData["Message"] = "Either permissions have not been set for this page or page does not exist.";
            TempData["InputString"] = url;

            return PartialView("ToolsError");
        }

        [HttpGet]
        public ActionResult UserPermissions()
        {
            return View("UserPermissions/Index");
        }

        [HttpGet]
        public ActionResult CheckUserPermissions(FindUserModel model)
        {
            var modelList = _permissionsControlService.CheckUserPermissions(model);

            if (!modelList.IsNullOrEmpty()) return PartialView("CheckUserPermissions", modelList);

            TempData["Message"] = "Either user has no permissions setup or this user does not exist.";
            TempData["InputString"] = model.EmailAddress + model.UserName;

            return PartialView("ToolsError");
        }

        [HttpGet]
        public ActionResult CheckPagesWithoutAuthor()
        {
            var modelList = _permissionsControlService.PagesWithoutAuthor();

            if (!modelList.IsNullOrEmpty()) return View("PagesWithoutAuthors", modelList);

            TempData["Message"] = "Unable to find pages without authors.";

            return PartialView("ToolsError");
        }

        [HttpGet]
        public ActionResult InboundLinks()
        {
            return View("InboundLinks/Index");
        }

        [HttpGet]
        public ActionResult FindInboundLinks(string url)
        {
            var modelList = _umbracoService.FindInboundLinks(url);

            if (modelList != null) return PartialView("InboundLinks/CheckInboundLinks", modelList);

            TempData["Message"] = "This page does not have any inbound links.";
            TempData["InputString"] = url;

            return PartialView("ToolsError");
        }

        //[HttpGet]
        //public ActionResult LookupAuthorPages(string userName)
        //{
        //    var model = new FindUserModel() {UserName = userName};
        //    var modelList = _permissionsControlService.CheckUserPermissions(model);

        //    return View("UserPermissions/CheckUserPermissions", "UserPermissions/Index", modelList);
        //}
    }
}