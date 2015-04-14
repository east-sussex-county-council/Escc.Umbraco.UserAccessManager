using System;
using System.Web.Mvc;

namespace ESCC.Umbraco.UserAccessManager.Utility
{
    /// <summary>
    /// Override Authorize to allow capture of and redirect of unauthorised access
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeRedirect : AuthorizeAttribute
    {
        public string RedirectUrl = "~/Home/Unauthorized";

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