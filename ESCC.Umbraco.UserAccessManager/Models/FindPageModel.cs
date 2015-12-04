namespace ESCC.Umbraco.UserAccessManager.Models
{
    public class FindPageModel
    {
        public string Url { get; set; }

        public int? NodeId { get; set; }

        public bool IsUrlRequest { get { return !string.IsNullOrWhiteSpace(Url); } }

        public bool IsNodeIdRequest { get { return NodeId != null; } }

        public bool IsValidRequest { get { return IsUrlRequest | IsNodeIdRequest; } }
    }
}