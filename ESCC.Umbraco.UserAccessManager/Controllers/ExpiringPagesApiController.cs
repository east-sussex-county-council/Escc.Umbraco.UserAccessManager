using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.WebPages;
using Escc.Umbraco.UserAccessManager.Models;
using Escc.Umbraco.UserAccessManager.Services;
using Escc.Umbraco.UserAccessManager.Services.Interfaces;
using Exceptionless;
using log4net;

namespace Escc.Umbraco.UserAccessManager.Controllers
{
    public class ExpiringPagesApiController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger("RollingFileAppender");
        private static Mutex _mutex;

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
        [HttpPost]
        public HttpResponseMessage CheckForExpiringNodesByUser()
        {
            HttpResponseMessage res;

            try
            {
                log.Info("Starting Process");
                // Only allow one instance to run at a time
                const string appName = "CheckForExpiringNodesByUser";
                bool createdNew;
                _mutex = new Mutex(true, appName, out createdNew);

                if (!createdNew)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                // Check that the correct credentials have been supplied
                var content = Request.Content.ReadAsStringAsync().Result;
                if (!Authentication.AuthenticateUser(content))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden);
                }

                // OK, carry on
                log.Info("Checking for expring nodes");
                res = GetExpiringNodesByUser();
            }
            catch (Exception ex)
            {
                log.Error("Process failed - check Exceptionless");
                new Exception(ex.ToString()).ToExceptionless().Submit();
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
            finally
            {
                _mutex.Dispose();
            }

            return res;
        }

        private HttpResponseMessage GetExpiringNodesByUser()
        {
            GetConfigSettings();

            _umbracoService = new UmbracoService();
            _emailService = new EmailService();

            var users = _umbracoService.GetExpiringPagesByUser(_noOfDaysFrom);

            log.Info("Starting expiry email process");
            // For each user:
            foreach (var user in users)
            {
                if (user.Pages.Any())
                {
                    try
                    {
                        SendEmail(user);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Failure sending email to:" + user.User.EmailAddress);
                        new Exception(ex.ToString()).ToExceptionless().Submit(); 
                    }
                }
            }

            // Check for pages expiring soon and email Web Staff
            var warningList = new List<UserPageModel>();
            var soonDate = DateTime.Now.AddDays(_emailWebStaffAtDays + 1);

            var expiringSoon = users.Where(u => u.Pages.Any(p => p.ExpiryDate <= soonDate));
            foreach (var expiring in expiringSoon)
            {
                // Add the specific pages that will expire soon ... not all of them!
                foreach (var expiringPage in expiring.Pages.Where(p => p.ExpiryDate <= soonDate))
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
                try
                {
                    SendWarningEmail(warningList.OrderBy(o => o.ExpiryDate).ToList());
                }
                catch (Exception ex)
                {
                    log.Error("Failure sending warning email to Webstaff - Check Exceptionless");
                    new Exception(ex.ToString()).ToExceptionless().Submit();
                }
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
            log.Info("Expiry Email Sent to: " + emailTo);
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
            log.Info("Warning Email Sent to: " + emailTo);
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