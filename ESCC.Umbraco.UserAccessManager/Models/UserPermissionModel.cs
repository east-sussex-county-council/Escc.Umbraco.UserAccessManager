namespace Escc.Umbraco.UserAccessManager.Models
{
    public class UserPermissionModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string EmailAddress { get; set; }

        public bool UserLocked { get; set; }

        public bool IsWebAuthor { get; set; }

        public string[] PagePermissions { get; set; }
    }
}