using System.Configuration;

namespace Escc.Umbraco.UserAccessManager.Services
{
    public class Authentication
    {
        public static bool AuthenticateUser(string credentials)
        {
            if (string.IsNullOrEmpty(credentials))
            {
                return false;
            }

            var separator = credentials.IndexOf('&');
            var nameParam = credentials.Substring(0, separator);
            var passwordParam = credentials.Substring(separator + 1);

            var name = nameParam.Split('=')[1];
            var password = passwordParam.Split('=')[1];

            return CheckCredentials(name, password);
        }

        private static bool CheckCredentials(string username, string password)
        {
            return username == ConfigurationManager.AppSettings["apiuser"] && password == ConfigurationManager.AppSettings["apikey"];
        }
    }
}