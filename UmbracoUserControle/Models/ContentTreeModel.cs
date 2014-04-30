using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UmbracoUserControl.Models
{
    public class ContentTreeModel
    {
        public int Id { get; set; }

        public int ParentId { get; set; }

        public int RootId { get; set; }

        public string Name { get; set; }

        public bool Published { get; set; }

        public DateTime PublishedDate { get; set; }
    }
}