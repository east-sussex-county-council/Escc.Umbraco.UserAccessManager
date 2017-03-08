using System;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace Escc.Umbraco.UserAccessManager.Utility
{
    /// <summary>
    /// Override Authorize to allow capture of and redirect of unauthorised access
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeRedirect : AuthorizeAttribute
    {
        public string RedirectUrl = "~/Home/Unauthorized";
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            IPrincipal user = httpContext.User;
            IIdentity identity = user.Identity;

            if (!identity.IsAuthenticated)
            {
                return false;
            }

            var allowedGroups = string.IsNullOrEmpty(Roles) ? new string[0] : Roles.Split(',');
            foreach (var allowedGroup in allowedGroups)
            {
                if (user.IsInRole(ConfigurationManager.AppSettings[allowedGroup]))
                {
                    return true;
                }
            }

            var userOverride = ConfigurationManager.AppSettings["UsersOverride"];
            if (!string.IsNullOrEmpty(userOverride))
            {
                var users = userOverride.Split(',');
                if (users.Any(x => x.ToLower() == user.Identity.Name.Replace("ESCC\\", "").ToLower()))
                { return true; }
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);

            if (filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.RequestContext.HttpContext.Response.Redirect(RedirectUrl);
            }
        }
    }
}