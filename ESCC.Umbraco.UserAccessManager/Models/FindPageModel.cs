namespace Escc.Umbraco.UserAccessManager.Models
{
    public class FindPageModel
    {
        public string Url { get; set; }

        public int? NodeId { get; set; }

        public bool IsUrlRequest { get { return !string.IsNullOrWhiteSpace(Url); } }

        public bool IsNodeIdRequest { get { return NodeId != null; } }

        public bool IsValidRequest { get { return IsUrlRequest | IsNodeIdRequest; } }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance in activity logs
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[Url:{0},NodeId:{1}]", Url, NodeId);
        }
    }
}