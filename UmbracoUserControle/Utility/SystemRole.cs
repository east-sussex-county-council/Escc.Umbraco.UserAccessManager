using System.Configuration;

namespace UmbracoUserControl.Utility
{
    public static class SystemRole
    {
        public static string WebServices = ConfigurationManager.AppSettings["SystemRole.WebServices"];
               
        public static string ServiceDesk = ConfigurationManager.AppSettings["SystemRole.ServiceDesk"];

        public static string AllAuthorised = WebServices + "," + ServiceDesk;
    }
}