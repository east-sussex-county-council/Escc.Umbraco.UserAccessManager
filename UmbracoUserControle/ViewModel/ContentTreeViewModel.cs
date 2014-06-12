using System;

namespace UmbracoUserControl.ViewModel
{
    public class ContentTreeViewModel
    {
        /// Info for Fancytree display
        public string title { get; set; }

        public int key { get; set; }

        public bool folder { get; set; }

        public bool lazy { get; set; }

        public bool selected { get; set; }

        /// Info for the page
        public int PageId { get; set; }

        public int ParentId { get; set; }

        public int RootId { get; set; }

        public string PageName { get; set; }

        public bool Published { get; set; }

        public DateTime PublishedDate { get; set; }

        /// Info for the User

        public int UserId { get; set; }

        public string FullName { get; set; }

        public string EmailAddress { get; set; }

        public bool isEditor { get; set; }
    }
}