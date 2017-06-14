using Escc.Umbraco.UserAccessManager.Models;
using Escc.Umbraco.UserAccessManager.Services;
using Escc.Umbraco.UserAccessManager.Services.Interfaces;
using Escc.Umbraco.UserAccessManager.Utility;
using Escc.Umbraco.UserAccessManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Web.Script.Serialization;
using System.Configuration;

namespace Escc.Umbraco.UserAccessManager.Controllers
{
    public class ExpiryEmailStatsController : Controller
    {
        private IDatabaseService _databaseService = new DatabaseService();

        #region ActionResults
        public ActionResult Index()
        {
            // Create the ViewModel and Lists
            ExpiryEmailStatsViewModel model = new ExpiryEmailStatsViewModel();
            List<ExpiryLogModel> EmailFailures = new List<ExpiryLogModel>();
            List<ExpiryLogModel> EmailSuccess = new List<ExpiryLogModel>();

            // Populate the Lists of ExpiryLogs
            EmailSuccess = _databaseService.GetExpiryLogSuccessDetails();
            EmailFailures = _databaseService.GetExpiryLogFailureDetails();

            // Find the last success and failure
            var MostRecentFailure = EmailFailures.Any() ? EmailFailures.OrderByDescending(x => x.ID).FirstOrDefault() : null;
            var MostRecentSuccess = EmailSuccess.OrderByDescending(x => x.ID).FirstOrDefault();

            // Assign variables to the ViewModel
            model.FailedEmails.Table = CreateTable(EmailFailures);
            model.SuccessfulEmails.Table = CreateTable(EmailSuccess);
            model.LastFailedEmail = MostRecentFailure;
            model.LastSuccessfulEmail = MostRecentSuccess;
            if (model.LastFailedEmail != null)
            {
                model.LastFailedEmail.PageCount = CountPages(model.LastFailedEmail.ID);
            }
            model.LastSuccessfulEmail.PageCount = CountPages(model.LastSuccessfulEmail.ID);
            model.ExpiringPages.Table = CreateExpiringPagesTable();

            // Return the Index view and pass it the ViewModel
            return View("Index", model);
        }

        public ActionResult GetPages(int ID)
        {
            // Create the ViewModel
            var model = new PagesViewModel();
            // Get the LogEntry         
            model.User = _databaseService.GetExpiryLogByID(ID);
            // Deserialize the Json pages string.
            List<UserPageModel> Pages = new JavaScriptSerializer().Deserialize<List<UserPageModel>>(model.User.Pages);

            // Create the Pages Datatable
            model.Pages.Table = new DataTable();
            model.Pages.Table.Columns.Add("ID", typeof(int));
            model.Pages.Table.Columns.Add("Name", typeof(string));
            model.Pages.Table.Columns.Add("Published Link", typeof(HtmlString));
            model.Pages.Table.Columns.Add("Edit Link", typeof(HtmlString));
            model.Pages.Table.Columns.Add("Expiry Date", typeof(DateTime));

            //Populate the Datatable
            foreach (var page in Pages)
            {
                HtmlString PublishedLink = new HtmlString(string.Format("<a href=\"{0}{1}\">{2}</a>", ConfigurationManager.AppSettings["PublishedURI"], page.PageUrl, page.PageUrl));
                HtmlString EditLink = new HtmlString(string.Format("<a href=\"{0}{1}{2}\">Edit</a>", ConfigurationManager.AppSettings["PublishedURI"], ConfigurationManager.AppSettings["EditURI"], page.PageId));
                model.Pages.Table.Rows.Add(page.PageId, page.PageName, PublishedLink, EditLink, page.ExpiryDate);
            }

            // Return the view model to the GetPages View
            return View(model);
        }
        #endregion

        #region Helper Methods
        private DataTable CreateExpiringPagesTable()
        {
            // Instantiate our lists and get all of the logs from the database
            List<ExpiryLogModel> ExpiryLogs = _databaseService.GetExpiryLogs();
            List<UserPageModel> ExpiringPages = new List<UserPageModel>();

            // Create the pages datatable
            var table = new DataTable();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Published Link", typeof(HtmlString));
            table.Columns.Add("Edit Link", typeof(HtmlString));
            table.Columns.Add("Authors", typeof(HtmlString));
            table.Columns.Add("Expiry Date", typeof(DateTime));
 
            // Go through each log
            foreach (var log in ExpiryLogs)
            {
                // Deserialize the pages for each log into a list of pages.
                List<UserPageModel> LogPages = new JavaScriptSerializer().Deserialize<List<UserPageModel>>(log.Pages);
                // for each page model in the pages list
                foreach (var page in LogPages)
                {
                    // if its expire date is greater than todays date
                    //if (page.ExpiryDate >= DateTime.Now)
                    //{
                    // if the page isn't already in the expiring pages list
                    if (!ExpiringPages.Any(x => x.PageId == page.PageId))
                    {
                        // add the page to the expiring pages list
                        page.Authors = new List<string>();
                        page.Authors.Add(log.EmailAddress);
                        ExpiringPages.Add(page);
                    }
                    else
                    {
                        ExpiringPages.Single(x => x.PageId == page.PageId).Authors.Add(log.EmailAddress);
                    }
                    //}
                }
            }

            // Populate the datatable with the pages from the expiring pages list
            foreach (var page in ExpiringPages)
            {
                HtmlString Authors = GetAuthorsHtmlString(page.Authors, page.PageId);
                HtmlString PublishedLink = new HtmlString(string.Format("<a href=\"{0}{1}\">{2}</a>", ConfigurationManager.AppSettings["PublishedURI"], page.PageUrl, page.PageUrl));
                HtmlString EditLink = new HtmlString(string.Format("<a href=\"{0}{1}{2}\">Edit</a>", ConfigurationManager.AppSettings["PublishedURI"], ConfigurationManager.AppSettings["EditURI"], page.PageId));
                table.Rows.Add(page.PageId, page.PageName, PublishedLink, EditLink, Authors, page.ExpiryDate);
            }

            // return the datatable
            return table;
        }

        private HtmlString GetAuthorsHtmlString(List<string> authors, int pageID)
        {
            var authorString = string.Format("<div class=\"dropdown\"><button class=\"btn btn-default dropdown-toggle\" type=\"button\" id=\"{0}\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"true\">Authors<span class=\"caret\"></span>  </button>  <ul class=\"dropdown-menu\" aria-labelledby=\"{1}\">", pageID, pageID);
            foreach (var author in authors)
            {
                authorString += string.Format("<li><a href=mailto:\"{0}\">{1}</a></li>", author, author);
            }
            authorString += string.Format("</ul></div>");

            HtmlString AuthorsHtmlString = new HtmlString(authorString);
            return AuthorsHtmlString;
        }

        private DataTable CreateTable(List<ExpiryLogModel> modelList)
        {
            //Create a new DataTable
            DataTable table = new DataTable();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Email", typeof(HtmlString));
            table.Columns.Add("Date", typeof(string));
            table.Columns.Add("Pages", typeof(HtmlString));

            //Populate the DataTable
            foreach (var model in modelList)
            {
                HtmlString Email = new HtmlString("<a href=mailto:" + model.EmailAddress + ">" + model.EmailAddress + "</a>");
                var pagesString = this.Url.Action("GetPages", "ExpiryEmailStats", new { ID = model.ID }, this.Request.Url.Scheme);
                HtmlString Pages = new HtmlString(string.Format("<a href=\"{0}\" class=\"btn btn-info\">Pages</a> <span class=\"badge badge-info\">{1}</span>", pagesString, CountPages(model.ID)));
                table.Rows.Add(model.ID, Email, model.DateAdded, Pages);
            }
            return table;
        }

        public int CountPages(int ID)
        {
            // Find the Log for the User ID
            var LogEntry = _databaseService.GetExpiryLogByID(ID);
            // Deserialize the Json pages string.
            List<UserPageModel> Pages = new JavaScriptSerializer().Deserialize<List<UserPageModel>>(LogEntry.Pages);
            return Pages.Count;
        }

        #endregion

    }
}