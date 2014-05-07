using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UmbracoUserControl.Models
{
    public class PermissionsModel
    {
        public int PermissionId { get; set; }

        public int PageId { get; set; }

        public int UserId { get; set; }

        public DateTime Created { get; set; }
    }
}