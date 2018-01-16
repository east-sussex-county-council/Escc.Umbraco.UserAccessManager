using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Escc.Umbraco.UserAccessManager.Models;
using Escc.Umbraco.UserAccessManager.Services.Interfaces;
using Exceptionless;
using Exceptionless.Extensions;

namespace Escc.Umbraco.UserAccessManager.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _umbracoSystem = ConfigurationManager.AppSettings["UmbracoSystem"];

        /// <summary>
        /// Sends and email to the given address
        /// </summary>
        /// <param name="emailTo">Address of user you wish to email</param>
        /// <param name="emailSubject">Subject line of email </param>
        /// <param name="emailBody">Body text of email</param>
        private void SmtpSendEmail(string emailTo, string emailSubject, string emailBody)
        {
            using (var client = new SmtpClient
            {
                UseDefaultCredentials = true
            })
            {
                using (var message = new MailMessage())
                {
                    message.To.Add(emailTo);
                    message.IsBodyHtml = true;
                    message.BodyEncoding = Encoding.UTF8;
                    message.Subject = emailSubject;
                    message.Body = emailBody;

                    try
                    {
                        // send the email
                        client.Send(message);
                    }
                    catch (SmtpException exception)
                    {
                        exception.ToExceptionless().Submit();
                    }
                }
            }
        }

        /// <summary>
        /// Email the password reset URL to the requestor
        /// </summary>
        /// <param name="model">PasswordResetModel - EmailAddress, UserId and UniqueResetId</param>
        /// <param name="url">root url for the site eg http://localhost:53201/ </param>
        public void PasswordResetEmail(PasswordResetModel model, string url)
        {
            var subject = string.Format("{0} – web author password", _umbracoSystem);

            var body = new StringBuilder();

            body.AppendLine("<p>Hello,</p>");
            body.AppendFormatLine("<p>This link takes you to the screen where you can set or change your password for the {0} content management system. Please note, this link will expire in 24 hours.</p>", _umbracoSystem);
            body.AppendFormatLine("<a href=\"{0}\">{0}</a>", string.Format("{0}/Admin/PasswordResetVerification?userId={1}&uniqueResetId={2}", url, model.UserId.ToString(), model.UniqueResetId));
            GetHelpText(body);

            SmtpSendEmail(model.EmailAddress, subject, body.ToString());
        }

        /// <summary>
        /// Send confirmation email to let the user know the password reset was successful
        /// </summary>
        /// <param name="model">PasswordResetModel - EmailAddress</param>
        public void PasswordResetConfirmationEmail(PasswordResetModel model)
        {
            var subject = string.Format("{0} – web author password successfully reset", _umbracoSystem);

            var body = new StringBuilder();

            body.AppendLine("<p>Hello,</p>");
            body.AppendFormatLine("<p>Your web author password has been successfully changed. You are now ready to start updating the {0}. Below is the website back office link where you need to login to edit your content.</p>", _umbracoSystem);
            body.AppendFormatLine("<a href=\"{0}\">{0}</a>", ConfigurationManager.AppSettings["UmbracoBackOfficeUrl"]);
            GetHelpText(body);

            SmtpSendEmail(model.EmailAddress, subject, body.ToString());
        }

        /// <summary>
        /// Get the text detailing where they can get help using the system
        /// </summary>
        /// <param name="body"></param>
        private void GetHelpText(StringBuilder body)
        {
            body.AppendLine("<p>If you need any help using the system, please refer to the user guides on the intranet.</p>");
            body.AppendLine("<p>Kind regards,<br/>Digital Services</p>");
            body.AppendFormatLine("<p>Guidance for web authors: <a href=\"{0}\">{0}</a>", ConfigurationManager.AppSettings["WebAuthorsGuidanceUrl"]);
            body.AppendFormatLine("<br/>Yammer: <a href=\"{0}\">{0}</a></p>", ConfigurationManager.AppSettings["WebAuthorsYammerUrl"]);
        }

        /// <summary>
        /// Email the admin team to inform them of the new account so they can setup permissions
        /// </summary>
        /// <param name="model">UmbracoUserModel - FullName, UserName and EmailAddress</param>
        public void CreateNewUserEmail(UmbracoUserModel model)
        {
            var subject = string.Format("{0} - new web author account created for {1}", _umbracoSystem, model.FullName);

            var body = new StringBuilder();

            body.AppendLine("<p>Hello,</p>");
            body.AppendFormatLine("<p>A new web author account has been created for {0}:</P>", model.FullName);
            body.AppendFormatLine("<p>Username: {0}<br/>Email address: {1}</p>", model.UserName, model.EmailAddress);
            body.AppendLine("<p>You can now go to the User Access Manager to set up the pages that this web author will be responsible for.</p>");

            var emailTo = ConfigurationManager.AppSettings["EmailTo"];

            SmtpSendEmail(emailTo, subject, body.ToString());
        }
    }
}