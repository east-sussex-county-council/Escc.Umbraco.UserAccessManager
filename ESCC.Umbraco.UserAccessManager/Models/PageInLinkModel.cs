using System.Collections.Generic;

namespace ESCC.Umbraco.UserAccessManager.Models
{
    /// <summary>
    /// Contains details of an inbound link
    /// </summary>
    public class PageInLinkModel
    {
        public PageInLinkModel()
        {
            FieldNames = new List<string>();
        }

        public int PageId { get; set; }

        public string PageName { get; set; }

        public List<string> FieldNames { get; set; }

        public string PageUrl { get; set; }
    }
}