using Castle.Core.Logging;
using PagedList;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using UmbracoUserControl.Models;
using UmbracoUserControl.Services;

namespace UmbracoUserControl.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserControlService userControlService;

        private ILogger Logger { get; set; }

        public AdminController(IUserControlService userControlService)
        {
            this.userControlService = userControlService;
        }

        [HttpPost]
        public ActionResult LookUpUser(FindUserModel model)
        {
            if (!ModelState.IsValid) return RedirectToAction("Index", "Home");

            return DisplayResults(model);
        }

        [HttpGet]
        public ActionResult DisplayResults(FindUserModel model)
        {
            if (model == null) return RedirectToAction("Index", "Home");

            var pageIndex = model.Page ?? 1;

            model.SearchResult = userControlService.LookupUsers(model).ToPagedList(pageIndex, 10);

            return View("UserLookup", model);
        }

        [HttpGet]
        public ActionResult InitiatePasswordReset(PasswordResetModel model)
        {
            if (Request.Url == null) return RedirectToAction("Index", "Home");

            var url = String.Format("http://{0}:{1}{2}", Request.Url.Host, Request.Url.Port, Request.ApplicationPath);

            var success = userControlService.InitiatePasswordReset(model, url);

            if (success)
            {
                TempData["Message"] = "Password reset proccess initiated, user will be emailed";
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult PasswordResetVerification(PasswordResetModel model)
        {
            return View("PasswordResetVerification", model);
        }

        [HttpPost]
        public ActionResult ResetPassword(PasswordResetModel model)
        {
            if (!ModelState.IsValid) return View("PasswordResetVerification", model);

            var success = userControlService.ResetPassword(model);

            if (success)
            {
                return RedirectToAction("DisplaySuccess", "Admin");
            }

            TempData["Message"] = "24-hour reset windows has elapsed, please contact ICT Service Desk to try again";

            return RedirectToAction("PasswordResetVerification", "Admin", model);
        }

        [HttpGet]
        public ActionResult InitiateUserCreation()
        {
            return View("CreateUser");
        }

        [HttpPost]
        public ActionResult CreateUser(UmbracoUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateUser", model);
            }

            var success = userControlService.CreateUser(model);

            return success ? RedirectToAction("InitiatePasswordReset", "Admin", model) : RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult DisableUser(UmbracoUserModel model)
        {
            var success = userControlService.ToggleLock(model);

            if (success)
            {
                return RedirectToAction("Index", "Home");
            }

            var findUserModel = new FindUserModel { EmailAddress = model.EmailAddress, UserName = model.UserName };

            return LookUpUser(findUserModel);
        }

        [HttpGet]
        public ActionResult DisplaySuccess()
        {
            return View("Success");
        }
    }
}