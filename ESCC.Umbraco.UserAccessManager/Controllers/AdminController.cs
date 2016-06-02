using System;
using System.Web.Mvc;
using Escc.Umbraco.UserAccessManager.Models;
using Escc.Umbraco.UserAccessManager.Services.Interfaces;
using Escc.Umbraco.UserAccessManager.Utility;
using PagedList;

namespace Escc.Umbraco.UserAccessManager.Controllers
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

            var urlScheme = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Host);
            var urlPort = Request.Url.Port;

            // Add port number if it is non-standard
            if (urlPort != 80 && urlPort != 443)
            {
                urlScheme = string.Format("{0}:{1}", urlScheme, Request.Url.Port);
            }

            var url = urlScheme;

            // Check that application path is not just "/"
            if (Request.ApplicationPath != "/")
            {
                url = String.Format("{0}{1}", url, Request.ApplicationPath);
            }

            var success = _userControlService.InitiatePasswordReset(model, url);

            if (success)
            {
                TempData["MsgKey"] = "PwdResetInitialised";
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

            TempData["MsgKey"] = "LinkNotValid";

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

            TempData["MsgKey"] = "LinkNotValid";

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
            if (user == null || user.Count > 0)
            {
                TempData["MsgKey"] = "EmailInUse";
                return View();
            }

            find.EmailAddress = null;
            find.UserName = model.UserName;
            user = _userControlService.LookupUsers(find);
            if (user.Count > 0)
            {
                TempData["MsgKey"] = "LoginInUse";
                return View();
            }

            var newUser = _userControlService.CreateUser(model);

            // Redmine #7903 - do not email user because no permissions have been setup yet
            //return (newUser != null) ? RedirectToAction("InitiatePasswordReset", "Admin", newUser) : RedirectToAction("Index", "Home");
            if (newUser == null) return RedirectToAction("Index", "Home");

            var findUser = new FindUserModel {EmailAddress = newUser.EmailAddress};
            return DisplayResults(findUser);
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