using System;
using System.Configuration;
using System.Net.Mail;
using ESCC.Umbraco.UserAccessManager.Models;
using ESCC.Umbraco.UserAccessManager.Services.Interfaces;

namespace ESCC.Umbraco.UserAccessManager.Services
{
    public class EmailService : IEmailService
    {
        private MailMessage _mail;

        /// <summary>
        /// Sends and email to the given address
        /// </summary>
        /// <param name="emailTo">Address of user you wish to email</param>
        /// <param name="emailSubject">Subject line of email </param>
        /// <param name="emailBody">Body text of email</param>
        private void SmtpSendEmail(string emailTo, string emailSubject, string emailBody)
        {
            _mail = new MailMessage();
            _mail.To.Add(emailTo);
            //mail.From = new MailAddress(ConfigurationManager.AppSettings["EmailFrom"]);
            _mail.Subject = emailSubject;
            _mail.Body = emailBody;
            _mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient()
            {
                //Added mailSettings section to web.config, so this is not needed
                //Host = ConfigurationManager.AppSettings["EmailHost"],
                //Port = int.Parse(ConfigurationManager.AppSettings["EmailPort"]),
                UseDefaultCredentials = true
            };
            //smtp.Credentials = new NetworkCredential("user", "pass");
            //smtp.Credentials = new System.Net.NetworkCredential();
            //smtp.EnableSsl = true;
            smtp.Send(_mail);
        }

        /// <summary>
        /// Email the password reset URL to the requestor
        /// </summary>
        /// <param name="model">PasswordResetModel - EmailAddress, UserId and UniqueResetId</param>
        /// <param name="url">root url for the site eg http://localhost:53201/ </param>
        public void PasswordResetEmail(PasswordResetModel model, string url)
        {
            var link = String.Format("<p>Click this link, or paste into a browser to reset your password:</p><p>{0}/Admin/PasswordResetVerification?userId={1}&uniqueResetId={2}</p>", url, model.UserId, model.UniqueResetId);

            SmtpSendEmail(model.EmailAddress, "Reset your password", link);
        }

        /// <summary>
        /// Send confirmation email to let the user know the password reset was successful
        /// </summary>
        /// <param name="model">PasswordResetModel - EmailAddress</param>
        public void PasswordResetConfirmationEmail(PasswordResetModel model)
        {
            SmtpSendEmail(model.EmailAddress, "Password successfully reset", "<p>Your password has been successfully changed.</p>");
        }

        /// <summary>
        /// Email the admin team to inform them of the new account so they can setup permissions
        /// </summary>
        /// <param name="model">UmbracoUserModel - FullName, UserName and EmailAddress</param>
        public void CreateNewUserEmail(UmbracoUserModel model)
        {
            var subject = string.Format("New account created for {0}", model.FullName);

            var body = string.Format("<p>A new account has been created for {0}:</P><p>Logon ID: {1}<br/>Email address: {2}</p><p>Please contact the user and setup appropriate permissions.</p>", model.FullName, model.UserName, model.EmailAddress);

            var emailTo = ConfigurationManager.AppSettings["EmailTo"];

            SmtpSendEmail(emailTo, subject, body);
        }
    }
}