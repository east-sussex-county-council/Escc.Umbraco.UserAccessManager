using System.Web.Http;

namespace ESCC.Umbraco.UserAccessManager
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration configuration)
        {
            configuration.Routes.MapHttpRoute(
                "API Default", 
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional }
            );
        }
    }
}