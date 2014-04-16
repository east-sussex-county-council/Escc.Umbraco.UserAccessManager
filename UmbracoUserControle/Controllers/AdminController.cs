using Castle.Core.Logging;
using Microsoft.ApplicationBlocks.ExceptionManagement;
using System;
using System.Web.Mvc;
using UmbracoUserControl.Models;
using UmbracoUserControl.Services;

namespace UmbracoUserControl.Controllers
{
    public class AdminController : Controller
    {
        private IUserControlService userControlService;
        private IUmbracoService umbracoService;

        public ILogger Logger { get; set; }

        public AdminController(IUmbracoService umbracoService, IUserControlService userControlService)
        {
            this.umbracoService = umbracoService;
            this.userControlService = userControlService;
        }

        [HttpPost]
        public ActionResult LookUpUser(UmbracoUserModel model)
        {
            if (ModelState.IsValid)
            {
                return DisplayResults(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult DisplayResults(UmbracoUserModel model)
        {
            try
            {
                var modelList = umbracoService.GetAllUsersByEmail(model.EmailAddress);

                return View("UsersByEmail", modelList);
            }
            catch (Exception ex)
            {
                // to do remove logging from here and add else where
                TempData["Message"] = "Error finding email address";

                Logger.ErrorFormat("{0} Email address {1} was not found - error message {2} - Stack trace {3} - inner exception {4}", DateTime.Now, model.EmailAddress, ex.Message, ex.StackTrace, ex.InnerException);

                ExceptionManager.Publish(ex);

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult InitiatePasswordReset(PasswordResetModel model)
        {
            var url = String.Format("http://{0}:{1}", Request.Url.Host, Request.Url.Port);

            bool success = userControlService.InitiatePasswordReset(model, url);

            if (success == true)
            {
                TempData["Message"] = "Password reset proccess initiated, user will be emailed";

                return RedirectToAction("Index", "Home");
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
            if (!ModelState.IsValid)
            {
                return View("PasswordResetVerification", model);
            }

            var success = userControlService.ResetPassword(model);

            if (success)
            {
                return RedirectToAction("DisplaySuccess", "Admin");
            }

            TempData["Message"] = "24-hour reset windows has elapsed, please contact ICT Service Desk to try again";

            return RedirectToAction("PasswordResetVerification", "Admin", model);
        }

        [HttpGet]
        public ActionResult InitiateUserCreation(UmbracoUserModel model)
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

            if (success)
            {
                return RedirectToAction("InitiatePasswordReset", "Admin", model);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult DisableUser(UmbracoUserModel model)
        {
            var success = userControlService.ToggleLock(model);

            if (success)
            {
                return RedirectToAction("Index", "Home");
            }

            return DisplayResults(model);
        }

        [HttpGet]
        public ActionResult DisplaySuccess()
        {
            return View("Success");
        }
    }
}