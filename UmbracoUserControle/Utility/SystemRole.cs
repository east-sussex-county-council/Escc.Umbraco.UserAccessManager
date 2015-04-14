using System.Configuration;

namespace UmbracoUserControl.Utility
{
    /// <summary>
    /// Derived from http://www.squarewidget.com/authorizationattribute-with-windows-authentication-in-mvc-4
    /// </summary>
    public static class SystemRole
    {
        public static string WebServices = ConfigurationManager.AppSettings["SystemRole.WebServices"];
               
        public static string ServiceDesk = ConfigurationManager.AppSettings["SystemRole.ServiceDesk"];

        public static string AllAuthorised = WebServices + "," + ServiceDesk;
    }
}