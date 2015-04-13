using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Castle.Core.Internal;
using UmbracoUserControl.Models;
using UmbracoUserControl.Services.Interfaces;
using UmbracoUserControl.Utility;
using UmbracoUserControl.ViewModel;

namespace UmbracoUserControl.Controllers
{
    [AuthorizeRedirect(Roles = SystemRole.WebServices)]
    public class PermissionsController : Controller
    {
        private readonly IUserControlService _userControlService;
        private readonly IPermissionsControlService _permissionsControlService;

        public PermissionsController(IUserControlService userControlService, IPermissionsControlService permissionsControlService)
        {
            _userControlService = userControlService;
            _permissionsControlService = permissionsControlService;
        }

        [HttpGet]
        public ActionResult Index(int id)
        {
            var model = _userControlService.LookupUserById(id);
            return View("Index", model);
        }

        [HttpGet]
        public ActionResult LookupContentTree(ContentTreeViewModel model)
        {
            return PartialView("ContentTree", model);
        }

        [HttpGet]
        public JsonResult PopTreeRootResult(ContentTreeViewModel model)
        {
            var modelList = _permissionsControlService.GetContentRoot(model);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PopTreeChildResult(ContentTreeViewModel model)
        {
            IList<ContentTreeViewModel> modelList = _permissionsControlService.GetContentChild(model);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ChangePermissionsResult(ContentTreeViewModel model)
        {
            if (model.selected)
            {
                var success = _permissionsControlService.SetContentPermissions(model);

                if (success)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var success = _permissionsControlService.RemoveContentPermissions(model);

                if (success)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckDestinationUser(FindUserModel model)
        {
            if (_userControlService.LookupUsers(model).IsNullOrEmpty()) return Json(false, JsonRequestBehavior.AllowGet);

            var user = _userControlService.LookupUsers(model).First();

            return Json(user, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CopyPermissionsForUser(int sourceId, int targetId)
        {
            var isRedirect = _permissionsControlService.ClonePermissions(sourceId, targetId);

            return Json(new
            {
                redirectUrl = Url.Action("Index", targetId),
                isRedirect
            });
        }
    }
}