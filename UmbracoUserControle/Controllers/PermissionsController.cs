using Castle.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UmbracoUserControl.Models;
using UmbracoUserControl.Services;
using UmbracoUserControl.Services.Interfaces;
using UmbracoUserControl.ViewModel;

namespace UmbracoUserControl.Controllers
{
    public class PermissionsController : Controller
    {
        private readonly IUserControlService userControlService;
        private readonly IPermissionsControlService permissionsControlService;

        public PermissionsController(IUserControlService userControlService, IPermissionsControlService permissionsControlService)
        {
            this.userControlService = userControlService;
            this.permissionsControlService = permissionsControlService;
        }

        public ActionResult Index(int id)
        {
            var model = userControlService.LookupUserById(id);

            return View(model);
        }

        public ActionResult LookupContentTree(ContentTreeViewModel model)
        {
            return PartialView("ContentTree", model);
        }

        public JsonResult PopTreeRootResult(ContentTreeViewModel model)
        {
            var modelList = permissionsControlService.GetContentRoot(model);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PopTreeChildResult(ContentTreeViewModel model)
        {
            var modelList = permissionsControlService.GetContentChild(model);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangePermissionsResult(ContentTreeViewModel model)
        {
            var success = permissionsControlService.SetContentPermissions(model);
            // to do
            return Json("");
        }
    }
}