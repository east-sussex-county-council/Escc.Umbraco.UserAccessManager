using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.WebPages;
using ESCC.Umbraco.UserAccessManager.Models;
using ESCC.Umbraco.UserAccessManager.Services;
using ESCC.Umbraco.UserAccessManager.Services.Interfaces;

namespace ESCC.Umbraco.UserAccessManager.Controllers
{
    public class ExpiringPagesApiController : ApiController
    {
        private IUmbracoService _umbracoService;
        private IEmailService _emailService;

        private string _webStaffEmail;
        private string _forceSendTo;
        private int _noOfDaysFrom;
        private int _emailWebStaffAtDays;

        public ExpiringPagesApiController()
        {
            
        }

        public ExpiringPagesApiController(IUmbracoService umbracoService, IEmailService emailService)
        {
            _umbracoService = umbracoService;
            _emailService = emailService;
        }

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage CheckForExpiringNodes()
        {
            GetConfigSettings();

            _umbracoService = new UmbracoService();
            _emailService = new EmailService();

            IList<ExpiringPageModel> expiringNodes = _umbracoService.GetExpiringPages(_noOfDaysFrom);

            // For each page:
            foreach (var expiringNode in expiringNodes)
            {
                //   Email Web Authors
                if (expiringNode.PageUsers.Any())
                {
                    foreach (var pageuser in expiringNode.PageUsers)
                    {
                        SendEmail(pageuser.EmailAddress, expiringNode, pageuser);
                    }

                    //   if only "n" day(s) left
                    var daysLeft = (expiringNode.ExpiryDate - DateTime.Now).Days;
                    if (daysLeft == _emailWebStaffAtDays)
                    {
                        SendEmail(_webStaffEmail, expiringNode);
                    }
                }
                else
                {
                    // no Web Authors assigned, so email WebStaff
                    SendEmail(_webStaffEmail, expiringNode);
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage CheckForExpiringNodesByUser()
        {
            GetConfigSettings();

            _umbracoService = new UmbracoService();
            _emailService = new EmailService();

            IList<UserPagesModel> users = _umbracoService.GetExpiringPagesByUser(_noOfDaysFrom);

            // For each user:
            foreach (var user in users)
            {
                if (user.Pages.Any())
                {
                    SendEmail(user);
                }

                //   Email Web Authors
                //if (user.PageUsers.Any())
                //{
                //    foreach (var pageuser in user.PageUsers)
                //    {
                //        SendEmail(pageuser.EmailAddress, user, pageuser);
                //    }

                //    //   if only "n" day(s) left
                //    var daysLeft = (user.ExpiryDate - DateTime.Now).Days;
                //    if (daysLeft == _emailWebStaffAtDays)
                //    {
                //        SendEmail(_webStaffEmail, user);
                //    }
                //}
                //else
                //{
                    // no Web Authors assigned, so email WebStaff
                    //SendEmail(user);
            //    }
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private void SendEmail(UserPagesModel userPages)
        {
            var emailTo = userPages.User.EmailAddress;

            // If "ForceEmailTo" is set, send all emails there instead (for Testing)
            if (!string.IsNullOrEmpty(_forceSendTo))
            {
                emailTo = _forceSendTo;
            }

            _emailService.UserPageExpiryEmail(emailTo, userPages);
        }

        private void GetConfigSettings()
        {
            _noOfDaysFrom = ConfigurationManager.AppSettings["NoOfDaysFrom"].AsInt(14);
            _emailWebStaffAtDays = ConfigurationManager.AppSettings["EmailWebStaffAtDays"].AsInt(3);
            _webStaffEmail = ConfigurationManager.AppSettings["WebStaffEmail"];
            _forceSendTo = ConfigurationManager.AppSettings["ForceSendTo"];
        }


        private void SendEmail(string emailTo, ExpiringPageModel contentNode)
        {
            // Construct a minimal UmbracouserModel
            var webStaff = new UmbracoUserModel
            {
                FullName = "Web Staff", 
                EmailAddress = _webStaffEmail
            };

            SendEmail(emailTo, contentNode, webStaff);
        }

        private void SendEmail(string emailTo, ExpiringPageModel contentNode, UmbracoUserModel pageUser)
        {
            // If "ForceEmailTo" is set, send all emails there instead (for Testing)
            if (!string.IsNullOrEmpty(_forceSendTo))
            {
                emailTo = _forceSendTo;
            }

            _emailService.PageExpiryWarningEmail(emailTo, contentNode, pageUser);
        }
    }
}