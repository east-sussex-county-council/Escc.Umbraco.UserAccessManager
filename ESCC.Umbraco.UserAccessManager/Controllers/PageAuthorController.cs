using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Castle.Core.Internal;
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

            if (!modelList.IsNullOrEmpty()) return PartialView("CheckPagePermissions", modelList);

            TempData["Message"] = "Either permissions have not been set for this page or page does not exist.";
            TempData["InputString"] = url;

            return PartialView("Error");
        }
    }
}