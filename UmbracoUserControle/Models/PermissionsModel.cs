using PetaPoco;
using System;

namespace UmbracoUserControl.Models
{
    [TableName("permissions")]
    [PrimaryKey("PermissionId")]
    public class PermissionsModel
    {
        public int PermissionId { get; set; }

        public int PageId { get; set; }

        public int UserId { get; set; }

        public DateTime? Created { get; set; }

        [Ignore]
        public int TargetId { get; set; }

        public string PageName { get; set; }

        public string FullName { get; set; }

        public string EmailAddress { get; set; }

        [Ignore]
        public string PagePath { get; set; }
    }
}