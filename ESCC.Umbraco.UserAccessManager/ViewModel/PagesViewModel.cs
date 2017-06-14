using Escc.Umbraco.UserAccessManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Escc.Umbraco.UserAccessManager.ViewModel
{
    public class PagesViewModel
    {
        public TableModel Pages { get; set; }
        public ExpiryLogModel User { get; set; }
        public PagesViewModel()
        {
            Pages = new TableModel("PagesTable");
        }
    }
}