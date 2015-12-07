using System;

namespace ESCC.Umbraco.UserAccessManager.Models
{
    public class PageModel
    {
        // Umbraco page (node)
        public int PageId { get; set; }

        public string PageName { get; set; }

        public string PagePath { get; set; }

        public string PageUrl { get; set; }

        public DateTime ExpiryDate { get; set; }
    }
}