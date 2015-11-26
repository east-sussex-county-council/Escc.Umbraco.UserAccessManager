using System.Collections.Generic;

namespace ESCC.Umbraco.UserAccessManager.Models
{
    public class PageLinksModel
    {
        public PageLinksModel()
        {
            InboundLinksLocal = new List<PageInLinkModel>();
            InboundLinksRedirect = new List<RedirectModel>();
            InboundLinksExternal = new List<InspyderLinkModel>();
        }

        public int PageId { get; set; }

        public string PageName { get; set; }

        public string PageUrl { get; set; }

        public List<PageInLinkModel> InboundLinksLocal { get; set; }
        public List<RedirectModel> InboundLinksRedirect { get; set; }
        public List<InspyderLinkModel> InboundLinksExternal { get; set; }
    }
}