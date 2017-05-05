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

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance in activity logs
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[UserName:{0},FullName:{1},EmailAddress:{2},UserId:{3},UserLocked:{4},IsWebAuthor:{5}]", UserName, FullName, EmailAddress, UserId, UserLocked, IsWebAuthor);
        }
    }
}