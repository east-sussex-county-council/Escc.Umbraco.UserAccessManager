using System.Web.Mvc;
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

        public JsonResult PopTreeChildResult(ContentTreeViewModel model)
        {
            var modelList = permissionsControlService.GetContentChild(model);

            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangePermissionsResult(ContentTreeViewModel model)
        {
            if (model.selected)
            {
                var success = permissionsControlService.SetContentPermissions(model);

                if (success)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                };
            }
            else
            {
                var success = permissionsControlService.RemoveContentPermissions(model);

                if (success)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                };
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CheckPermissionsForUser(int id)
        {
            var success = permissionsControlService.CheckUserPermissions(id);
            // not doing anything with succes now
            // think on what to do with it
            return Index(id);
        }
    }
}