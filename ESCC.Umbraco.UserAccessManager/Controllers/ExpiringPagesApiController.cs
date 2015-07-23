using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using System.Web.WebPages;
using ESCC.Umbraco.UserAccessManager.Models;
using ESCC.Umbraco.UserAccessManager.Services;
using ESCC.Umbraco.UserAccessManager.Services.Interfaces;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.WebApi;

namespace ESCC.Umbraco.UserAccessManager.Controllers
{
    public class ExpiringPagesApiController : ApiController
    {
        private IUmbracoService _umbracoService;
        private string _forceSendTo;
        private string _webAuthorUserType;
        private int _noOfDaysFrom;

        public ExpiringPagesApiController()
        {
            
        }

        public ExpiringPagesApiController(IUmbracoService umbracoService)
        {
            _umbracoService = umbracoService;
        }

        [HttpGet]
        public HttpResponseMessage CheckForExpiringNodes()
        {
            GetConfigSettings();

            _umbracoService = new UmbracoService();

            IList<ExpiringPageModel> expiringNodes = _umbracoService.GetExpiringPages(_noOfDaysFrom);

            // For each page:
            foreach (var expiringNode in expiringNodes)
            {
                //   Email Web Authors
                //   if no Web Authors, or only 1 day left
                //      Email WebStaff
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private void GetConfigSettings()
        {
            _noOfDaysFrom = ConfigurationManager.AppSettings["NoOfDaysFrom"].AsInt(14);
            _webAuthorUserType = ConfigurationManager.AppSettings["WebAuthorUserType"];
            _forceSendTo = ConfigurationManager.AppSettings["ForceSendTo"];
        }


        private void SendEmail(string emailTo, IContent contentNode)
        {
            var subject = string.Format("Page expiry - {0}", contentNode.Name);
            var body = "page about to expire";

            // If "ForceEmailTo" is set, send all emails there instead (for Testing)
            if (!string.IsNullOrEmpty(_forceSendTo))
            {
                emailTo = _forceSendTo;
            }

            using (var client = new SmtpClient())
            {
                using (var message = new MailMessage())
                {
                    message.To.Add(emailTo);
                    message.IsBodyHtml = true;
                    message.Subject = subject;
                    message.Body = body.ToString();

                    try
                    {
                        // send the email
                        client.Send(message);
                    }
                    catch (SmtpException exception)
                    {
                        throw;
                        //exception.ToExceptionless().Submit();
                    }
                }
            }

        }
    }
}