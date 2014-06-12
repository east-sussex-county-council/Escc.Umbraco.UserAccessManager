using System;
using System.Configuration;
using System.Net.Mail;
using UmbracoUserControl.Models;
using UmbracoUserControl.Services.Interfaces;

namespace Umbraco7._0._0.Services
{
    public class EmailService : IEmailService
    {
        private MailMessage mail;
        private IDatabaseService databaseService;

        public EmailService(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        /// <summary>
        /// Sends and email to the given address
        /// </summary>
        /// <param name="emailTo">Address of user you wish to email</param>
        /// <param name="emailSubject">Subject line of email </param>
        /// <param name="emailBody">Body text of email</param>
        private void SMTPSendEmail(string emailTo, string emailSubject, string emailBody)
        {
            mail = new MailMessage();
            mail.To.Add(emailTo);
            mail.From = new MailAddress(ConfigurationManager.AppSettings["EmailFrom"]);
            mail.Subject = emailSubject;
            mail.Body = emailBody;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient()
            {
                Host = ConfigurationManager.AppSettings["EmailHost"],
                Port = int.Parse(ConfigurationManager.AppSettings["EmailPort"]),
                UseDefaultCredentials = true
            };
            //smtp.Credentials = new NetworkCredential("user", "pass");
            //smtp.Credentials = new System.Net.NetworkCredential();
            //smtp.EnableSsl = true;
            smtp.Send(mail);
        }

        /// <summary>
        /// Email the password reset URL to the requestor
        /// </summary>
        /// <param name="model">PasswordResetModel - EmailAddress, UserId and UniqueResetId</param>
        /// <param name="url">root url for the site eg http://localhost:53201/ </param>
        public void PasswordResetEmail(PasswordResetModel model, string url)
        {
            var link = String.Format("{0}/Admin/PasswordResetVerification?userId={1}&uniqueResetId={2}", url, model.UserId, model.UniqueResetId);

            SMTPSendEmail(model.EmailAddress, "subject text", "body text " + link);
        }

        /// <summary>
        /// Send confirmation email to let the user know the password reset was successful
        /// </summary>
        /// <param name="model">PasswordResetModel - EmailAddress</param>
        public void PasswordResetConfirmationEmail(PasswordResetModel model)
        {
            SMTPSendEmail(model.EmailAddress, "subject text", "body text");
        }

        /// <summary>
        /// Email the admin team to inform them of the new account so they can setup permissions
        /// </summary>
        /// <param name="model">UmbracoUserModel - FullName, UserName and EmailAddress</param>
        public void CreateNewUserEmail(UmbracoUserModel model)
        {
            var subject = string.Format("New account created for {0}", model.FullName);

            var body = string.Format("A new account have been created for {0}, Username {1} Email {2}. <br> Please contract the user and setup appropriate permissions", model.FullName, model.UserName, model.EmailAddress);

            var emailTo = ConfigurationManager.AppSettings["EmailTo"];

            SMTPSendEmail(emailTo, subject, body);
        }
    }
}