using System;
using System.Collections.Generic;

namespace Escc.Umbraco.UserAccessManager.Models
{
    public class ExpiringPageModel
    {
        public int PageId { get; set; }

        public string PageName { get; set; }

        public string PagePath { get; set; }

        public DateTime ExpiryDate { get; set; }

        public IList<UmbracoUserModel> PageUsers { get; set; }
    }
}