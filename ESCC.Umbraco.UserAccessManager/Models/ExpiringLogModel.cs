using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Escc.Umbraco.UserAccessManager.Models
{
    public class ExpiryLogModel
    {
        public int ID { get; set; }
        public string EmailAddress { get; set; }
        public DateTime DateAdded { get; set; }
        public bool EmailSuccess { get; set; }
        public string Pages { get; set; }
        public int PageCount { get; set; }

        public ExpiryLogModel(int id, string emailAddress, DateTime dateAdded, bool emailSuccess, string pages)
        {
            ID = id;
            EmailAddress = emailAddress;
            DateAdded = dateAdded;
            EmailSuccess = emailSuccess;
            Pages = pages;
        }

        public ExpiryLogModel()
        {

        }
    }
}