using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UmbracoUserControl.Models
{
    [PetaPoco.TableName("permissions")]
    [PetaPoco.PrimaryKey("PermissionId")]
    public class PermissionsModel
    {
        public int PermissionId { get; set; }

        public int PageId { get; set; }

        public int UserId { get; set; }

        public DateTime? Created { get; set; }

        [PetaPoco.Ignore]
        public int TargetId { get; set; }
    }
}