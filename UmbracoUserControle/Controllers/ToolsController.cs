using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CheckPagePermissions(string url)
        {
            var modelList = permissionsControlService.CheckPagePermissions(url);

            return PartialView("CheckPagePermissions", modelList);
        }
    }
}