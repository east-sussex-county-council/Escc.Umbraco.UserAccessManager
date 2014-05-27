using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UmbracoUserControl.Models
{
    public class FindUserModel
    {
        public string EmailAddress { get; set; }

        public string UserName { get; set; }

        public IPagedList<UmbracoUserModel> SearchResult { get; set; }

        public int? Page { get; set; }

        public bool IsUserRequest { get { return !string.IsNullOrWhiteSpace(UserName); } }

        public bool IsEmailRequest { get { return !string.IsNullOrWhiteSpace(EmailAddress); } }

        public bool IsValidRequest { get { return IsEmailRequest | IsUserRequest; } }
    }
}