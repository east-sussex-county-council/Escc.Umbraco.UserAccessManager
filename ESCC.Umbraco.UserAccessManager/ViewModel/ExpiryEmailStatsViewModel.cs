using Escc.Umbraco.UserAccessManager.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Escc.Umbraco.UserAccessManager.ViewModel
{
    public class ExpiryEmailStatsViewModel
    {
        public TableModel SuccessfulEmails { get; set; }
        public TableModel FailedEmails { get; set; }
        public TableModel ExpiringPages { get; set; }

        public ExpiryEmailStatsViewModel()
        {
            SuccessfulEmails = new TableModel("SuccessTable");
            FailedEmails = new TableModel("FailedTable");
            ExpiringPages = new TableModel("Expiring Table");
        }
    }
}