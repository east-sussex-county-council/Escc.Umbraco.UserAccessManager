using System;

namespace ESCC.Umbraco.UserAccessManager.Models
{
    public class RedirectModel
    {
        public int RedirectId { get; set; }

        public string Pattern { get; set; }

        public string Destination { get; set; }

        public int Type { get; set; }

        public string Comment { get; set; }

        public DateTime DateCreated { get; set; }
    }
}