namespace Escc.Umbraco.UserAccessManager.Models
{
    public class UmbracoUserModel
    {
        public string UserName { get; set; }

        public string FullName { get; set; }

        public string EmailAddress { get; set; }

        public int UserId { get; set; }

        public bool UserLocked { get; set; }

        public bool IsWebAuthor { get; set; }
    }
}