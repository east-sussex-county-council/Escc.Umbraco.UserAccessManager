using Castle.Core.Internal;
using Microsoft.Ajax.Utilities;
using System.Linq;
using System.Web.Mvc;
using UmbracoUserControl.Models;
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

        [HttpGet]
        public ActionResult Index(int id)
        {
            var model = userControlService.LookupUserById(id);

            return View("Index", model);
        }

        [HttpGet]
        public ActionResult LookupContentTree(ContentTreeViewModel model)
        {
            return PartialView("ContentTree", model);
        }

        public JsonResult PopTreeRootResult(ContentTreeViewModel model)
        {
            var modelList = permissionsControlService.GetContentRoot(model);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PopTreeChildResult(ContentTreeViewModel model)
        {
            var modelList = permissionsControlService.GetContentChild(model);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ChangePermissionsResult(ContentTreeViewModel model)
        {
            if (model.selected)
            {
                var success = permissionsControlService.SetContentPermissions(model);

                if (success)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var success = permissionsControlService.RemoveContentPermissions(model);

                if (success)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CheckPermissionsForUser(int id)
        {
            var success = permissionsControlService.SyncUserPermissions(id);

            if (!success)
            {
                TempData["Message"] = "An error has occured - Tree has not been updated";
            }
            //this needs to be looked at on failour
            return Index(id);
        }

        [HttpGet]
        public JsonResult CheckDestinationUser(FindUserModel model)
        {
            if (!userControlService.LookupUsers(model).IsNullOrEmpty())
            {
                var user = userControlService.LookupUsers(model).First();

                return Json(user, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CopyPermissionsForUser(int sourceId, int targetId)
        {
            var isRedirect = permissionsControlService.ClonePermissions(sourceId, targetId);

            return Json(new
            {
                redirectUrl = Url.Action("Index", targetId),
                isRedirect
            });
        }
    }
}