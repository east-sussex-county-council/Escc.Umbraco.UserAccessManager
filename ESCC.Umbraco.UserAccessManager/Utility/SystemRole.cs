using System.Configuration;

namespace Escc.Umbraco.UserAccessManager.Utility
{
    /// <summary>
    /// Derived from http://www.squarewidget.com/authorizationattribute-with-windows-authentication-in-mvc-4
    /// Define the individual roles here and combine into groups as required.
    /// </summary>
    public static class SystemRole
    {
        public const string WebServices = "SystemRole.WebServices";
               
        public const string ServiceDesk = "SystemRole.ServiceDesk";

        public const string AllAuthorised = WebServices + "," + ServiceDesk;
    }
}