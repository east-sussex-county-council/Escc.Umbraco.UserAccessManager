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

            body.AppendLine("<p>Hello Web Staff,</p>");
            body.AppendFormatLine("<p>A new web author account has been created for {0}:</P>", model.FullName);
            body.AppendFormatLine("<p>Username: {0}<br/>Email address: {1}</p>", model.UserName, model.EmailAddress);
            body.AppendLine("<p>You can now go to the User Access Manager to set up the pages that this web author will be responsible for.</p>");

            var emailTo = ConfigurationManager.AppSettings["EmailTo"];

            SmtpSendEmail(emailTo, subject, body.ToString());
        }

        /// <summary>
        /// Send expiry warning emails to users (Web Authors) of pages about to expire
        /// </summary>
        /// <param name="emailTo">
        /// who to send the email to
        /// </param>
        /// <param name="userPages">
        /// User details and list of expiring pages for this user
        /// </param>
        public void UserPageExpiryEmail(string emailTo, UserPagesModel userPages)
        {
            var siteUri = ConfigurationManager.AppSettings["SiteUri"];

            foreach (var item in userPages.Pages)
            {
                if(item.PageUrl == null || item.PageUrl == "#")
                {
                    item.PageUrl = "This page is not visible on the live site.";
                }
            }

            var subject = string.Format("ACTION: Your {0} pages expire in under 14 days", _umbracoSystem);
            var body = new StringBuilder();

            body.AppendFormatLine("<p>Your {0} pages will expire within the next two weeks. After this they will no longer be available to the public. The dates for each page are given below.</p>", _umbracoSystem);
            body.AppendLine("<p>Note, you should always use Google Chrome to update your pages. If your default web browser is Internet Explorer, you will need to right-click and copy and paste the links below into Chrome instead.</p>");
            body.AppendLine("<p>After you’ve logged in, click on each page below and:</p>");
            body.AppendLine("<ul>");
            body.AppendLine("<li>check they are up to date</li>");
            body.AppendLine("<li>check the information is still needed</li>");
            body.AppendLine("<li>go to Properties tab and use the calendar to set a new date in the 'Unpublish at' box</li>");
            body.AppendLine("<li>then click 'Save and publish'.</li>");
            body.AppendLine("</ul>");
            body.AppendLine("<p>For details on updating your pages, see <a href=\"" + ConfigurationManager.AppSettings["WebAuthorsGuidanceUrl"] + "\">Guidance for web authors</a>.</p>");

            var otherTitle = "Expiring Pages:";
            var warningDate = DateTime.Now.AddDays(2);
            var lastWarningPages = userPages.Pages.Where(d => d.ExpiryDate <= warningDate).ToList();
            if (lastWarningPages.Any())
            {
                body.AppendLine("<strong>Pages Expiring Tomorrow:</strong>");
                body.AppendLine("<ol>");
                foreach (var page in lastWarningPages)
                {
                    var linkUrl = string.Format("{0}#/content/content/edit/{1}", siteUri, page.PageId);
                    body.Append("<li>");
                    body.AppendFormat("<a href=\"{0}\">{1}</a> (expires {2}, {3})", linkUrl, page.PageName, page.ExpiryDate.Value.ToLongDateString(), page.ExpiryDate.Value.ToShortTimeString());
                    body.AppendFormat("<br/>{0}", page.PageUrl);
                    body.Append("</li>");
                }
                body.AppendLine("</ol>");

                otherTitle = "Other Pages:";
            }

            // Process remaining pages
            var nonWarningPages = userPages.Pages.Where(d => d.ExpiryDate > warningDate).ToList();
            if (nonWarningPages.Any())
            {
                body.AppendFormatLine("<strong>{0}</strong>", otherTitle);
                body.AppendLine("<ol>");
                foreach (var page in nonWarningPages)
                {
                    var linkUrl = string.Format("{0}#/content/content/edit/{1}", siteUri, page.PageId);
                    body.Append("<li>");
                    body.AppendFormat("<a href=\"{0}\">{1}</a> (expires {2}, {3})", linkUrl, page.PageName, page.ExpiryDate.Value.ToLongDateString(), page.ExpiryDate.Value.ToShortTimeString());
                    body.AppendFormat("<br/>{0}", page.PageUrl);
                    body.Append("</li>");
                }
                body.AppendLine("</ol>");
            }

            var neverExpiringPages = userPages.Pages.Where(d => d.ExpiryDate == null).ToList();
            if (neverExpiringPages.Any() && (nonWarningPages.Any() || lastWarningPages.Any()))
            {
                body.AppendLine("<strong>Pages Never Expiring:</strong>");
                body.AppendLine("<p>As these pages never expire, its important to check them periodically.<p>");
                body.AppendLine("<p>After you’ve logged in, click on each page below and:</p>");
                body.AppendLine("<ul>");
                body.AppendLine("<li>check they are up to date</li>");
                body.AppendLine("<li>check the information is still needed</li>");
                body.AppendLine("<li>then click 'Save and publish'.</li>");
                body.AppendLine("</ul>");
                body.AppendLine("<p>You don't need to worry about setting any dates</p>");
                body.AppendLine("<ol>");
                foreach (var page in neverExpiringPages)
                {
                    var linkUrl = string.Format("{0}#/content/content/edit/{1}", siteUri, page.PageId);
                    body.Append("<li>");
                    body.AppendFormat("<a href=\"{0}\">{1}</a>", linkUrl, page.PageName);
                    body.AppendFormat("<br/>{0}", page.PageUrl);
                    body.Append("</li>");
                }
                body.AppendLine("</ol>");
            }
            if(lastWarningPages.Any() || nonWarningPages.Any())
            {
                SmtpSendEmail(emailTo, subject, body.ToString());
            }
        }

        /// <summary>
        /// Send email to WebStaff highlighting pages that will expire very soon (period set in web.config)
        /// </summary>
        /// <param name="emailTo">
        /// Web Staff email address
        /// </param>
        /// <param name="userPages">
        /// List of pages that will expire soon
        /// </param>
        /// <param name="emailWebStaffAtDays">
        /// Number of days before page expiry
        /// </param>
        public void UserPageLastWarningEmail(string emailTo, List<UserPageModel> userPages, int emailWebStaffAtDays)
        {
            var siteUri = ConfigurationManager.AppSettings["SiteUri"];

            var subject = string.Format("ACTION: The following {0} pages expire in under {1} days", _umbracoSystem, emailWebStaffAtDays);
            var body = new StringBuilder();

            body.AppendFormatLine("<p>These {0} pages will expire within the next {1} days. After this they will no longer be available to the public.</p>", _umbracoSystem, emailWebStaffAtDays.ToString());
            body.AppendLine("<p>Note, you should always use Google Chrome to update your pages. If your default web browser is Internet Explorer, you will need to right-click and copy and paste the links below into Chrome instead.</p>");
            body.AppendLine("<p>After you’ve logged in, click on each page below and:</p>");
            body.AppendLine("<ul>");
            body.AppendLine("<li>check they are up to date</li>");
            body.AppendLine("<li>check the information is still needed</li>");
            body.AppendLine("<li>go to Properties tab and use the calendar to set a new date in the 'Unpublish at' box</li>");
            body.AppendLine("<li>then click 'Save and publish'.</li>");
            body.AppendLine("</ul>");
            body.AppendLine("<p>For details on updating your pages, see <a href=\"" + ConfigurationManager.AppSettings["WebAuthorsGuidanceUrl"] + "\">Guidance for web authors</a>.</p>");

            // Process remaining pages
            body.AppendLine("<strong>Expiring Pages:</strong>");
            body.AppendLine("<ol>");
            foreach (var page in userPages)
            {
                var linkUrl = string.Format("{0}#/content/content/edit/{1}", siteUri, page.PageId);
                body.Append("<li>");
                body.AppendFormat("<a href=\"{0}\">{1}</a> (expires {2}, {3})", linkUrl, page.PageName, page.ExpiryDate.Value.ToLongDateString(), page.ExpiryDate.Value.ToShortTimeString());
                body.AppendFormat("<br/>{0}", page.PageUrl);
                body.Append("</li>");
            }
            body.AppendLine("</ol>");

            SmtpSendEmail(emailTo, subject, body.ToString());
        }

    }
}