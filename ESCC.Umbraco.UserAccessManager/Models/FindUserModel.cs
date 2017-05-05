using PagedList;

namespace Escc.Umbraco.UserAccessManager.Models
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

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance in activity logs
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[EmailAddress:{0},UserName:{1}]", EmailAddress, UserName);
        }
    }
}