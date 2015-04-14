using PagedList;
using System;
using System.Web.Mvc;
using UmbracoUserControl.Models;
using UmbracoUserControl.Services.Interfaces;
using UmbracoUserControl.Utility;

namespace UmbracoUserControl.Controllers
{
    [AuthorizeRedirect(Roles = SystemRole.WebServices)]
    public class AdminController : Controller
    {
        private readonly IUserControlService _userControlService;

        public AdminController(IUserControlService userControlService)
        {
            _userControlService = userControlService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRedirect(Roles = SystemRole.AllAuthorised)]
        public ActionResult LookUpUser(FindUserModel model)
        {
            if (!ModelState.IsValid) return RedirectToAction("Index", "Home");

            // Either an email address or user name must be supplied
            if (string.IsNullOrEmpty(model.EmailAddress) && string.IsNullOrEmpty(model.UserName))
            {
                return RedirectToAction("Index", "Home");
            }

            return DisplayResults(model);
        }

        [HttpGet]
        [AuthorizeRedirect(Roles = SystemRole.AllAuthorised)]
        public ActionResult DisplayResults(FindUserModel model)
        {
            if (model == null) return RedirectToAction("Index", "Home");

            var pageIndex = model.Page ?? 1;

            model.SearchResult = _userControlService.LookupUsers(model).ToPagedList(pageIndex, 10);

            return View("UserLookup", model);
        }

        [HttpGet]
        [AuthorizeRedirect(Roles = SystemRole.AllAuthorised)]
        public ActionResult InitiatePasswordReset(PasswordResetModel model)
        {
            if (Request.Url == null) return RedirectToAction("Index", "Home");

            var url = String.Format("http://{0}:{1}{2}", Request.Url.Host, Request.Url.Port, Request.ApplicationPath);

            var success = _userControlService.InitiatePasswordReset(model, url);

            if (success)
            {
                TempData["Message"] = "Password reset proccess initiated, user has been emailed";
            }

            return DisplayResults(new FindUserModel { EmailAddress = model.EmailAddress });
            //return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult PasswordResetVerification(PasswordResetModel model)
        {
            // Check if this is a valid link
            var validRequests = _userControlService.CheckResetDetails(model);

            if (validRequests)
            {
                return View("PasswordResetVerification", model);
            }

            TempData["Message"] = "This link is no longer valid, please contact ICT Service Desk to try again";

            return View("PasswordResetError", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult ResetPassword(PasswordResetModel model)
        {
            if (!ModelState.IsValid) return View("PasswordResetVerification", model);

            var success = _userControlService.ResetPassword(model);

            if (success)
            {
                return RedirectToAction("DisplaySuccess", "Admin");
            }

            TempData["Message"] = "This link is no longer valid, please contact ICT Service Desk to try again";

            return RedirectToAction("PasswordResetVerification", "Admin", model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult DisplaySuccess()
        {
            return View("Success");
        }

        [HttpGet]
        public ActionResult InitiateUserCreation()
        {
            return View("CreateUser");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRedirect(Roles = SystemRole.WebServices)]
        public ActionResult CreateUser(UmbracoUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateUser", model);
            }

            // Check that User does not already exist
            FindUserModel find = new FindUserModel {EmailAddress = model.EmailAddress, UserName = null};
            var user = _userControlService.LookupUsers(find);
            if (user.Count > 0)
            {
                TempData["Message"] = "Email address already being used";
                return View();
            }

            find.EmailAddress = null;
            find.UserName = model.UserName;
            user = _userControlService.LookupUsers(find);
            if (user.Count > 0)
            {
                TempData["Message"] = "Logon ID already being used";
                return View();
            }


            var success = _userControlService.CreateUser(model);

            return success ? RedirectToAction("InitiatePasswordReset", "Admin", model) : RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AuthorizeRedirect(Roles = SystemRole.WebServices)]
        public ActionResult DisableUser(UmbracoUserModel model)
        {
            var success = _userControlService.ToggleLock(model);

            if (success)
            {
                return DisplayResults(new FindUserModel {EmailAddress = model.EmailAddress});
            }

            var findUserModel = new FindUserModel { EmailAddress = model.EmailAddress, UserName = model.UserName };

            return LookUpUser(findUserModel);
        }
    }
}