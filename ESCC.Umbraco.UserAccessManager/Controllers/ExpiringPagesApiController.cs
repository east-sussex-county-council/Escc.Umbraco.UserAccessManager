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
        public HttpResponseMessage CheckForExpiringNodesByUser()
        {
            GetConfigSettings();

            _umbracoService = new UmbracoService();
            _emailService = new EmailService();

            var users = _umbracoService.GetExpiringPagesByUser(_noOfDaysFrom);

            // For each user:
            foreach (var user in users)
            {
                if (user.Pages.Any())
                {
                    SendEmail(user);
                }
            }

            // Check for pages expiring soon and email Web Staff
            var warningList = new List<UserPageModel>();

            var expiringSoon = users.Where(u => u.Pages.Any(p => p.ExpiryDate <= DateTime.Now.AddDays(_emailWebStaffAtDays + 1)));
            foreach (var expiring in expiringSoon)
            {
                foreach (var expiringPage in expiring.Pages)
                {
                    // Check we haven't already added this page to the list
                    if (warningList.All(n => n.PageId != expiringPage.PageId))
                    {
                        warningList.Add(expiringPage);
                    }
                }
            }

            if (warningList.Any())
            {
                SendWarningEmail(warningList);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Send page Expiry email, one per Web Author, listing all expiring pages
        /// </summary>
        /// <param name="userPages">
        /// Array of expiring pages
        /// </param>
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

        /// <summary>
        /// Send warning email to Web Staff listing pages that will expire within the defined period (web.config)
        /// </summary>
        /// <param name="warningPages">
        /// List of pages due to expire
        /// </param>
        private void SendWarningEmail(List<UserPageModel> warningPages)
        {
            var emailTo = _webStaffEmail;

            // If "ForceEmailTo" is set, send all emails there instead (for Testing)
            if (!string.IsNullOrEmpty(_forceSendTo))
            {
                emailTo = _forceSendTo;
            }

            _emailService.UserPageLastWarningEmail(emailTo, warningPages, _emailWebStaffAtDays);            
        }

        private void GetConfigSettings()
        {
            _noOfDaysFrom = ConfigurationManager.AppSettings["NoOfDaysFrom"].AsInt(14);
            _emailWebStaffAtDays = ConfigurationManager.AppSettings["EmailWebStaffAtDays"].AsInt(3);
            _webStaffEmail = ConfigurationManager.AppSettings["WebStaffEmail"];
            _forceSendTo = ConfigurationManager.AppSettings["ForceSendTo"];
        }
    }
}