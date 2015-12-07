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
            PageUsersModel modelList = _permissionsControlService.CheckPagePermissions(url);

            // If nodelList is null then the page does not exist in Umbraco
            if (modelList == null)
            {
                TempData["Message"] = "Sorry, that page does not exist.";

                return PartialView("Error");
            }

            // Remove any users who are currently disabled in Umbraco
            modelList.Users = modelList.Users.Where(u => u.UserLocked == false).ToList();

            // One or more Web Authors have been given permission to edit this page
            return PartialView("CheckPagePermissions", modelList);
        }
    }
}