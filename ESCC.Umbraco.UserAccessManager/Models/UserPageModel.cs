using System;
using System.Collections.Generic;

namespace Escc.Umbraco.UserAccessManager.Models
{
    public class UserPageModel
    {
        public int PageId { get; set; }

        public string PageName { get; set; }

        public string PagePath { get; set; }

        public string PageUrl { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public List<string> Authors { get; set; }
    }
}