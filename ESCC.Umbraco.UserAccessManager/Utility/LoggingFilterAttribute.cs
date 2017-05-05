using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Escc.Umbraco.UserAccessManager.Utility
{
    public class LoggingFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Access to the log4Net logging object
        /// </summary>
        protected static readonly log4net.ILog log =
          log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (log.IsInfoEnabled)
            {
                var message = new StringBuilder();
                message.Append(filterContext.HttpContext.User.Identity.Name)
                    .Append(" executing ").Append(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName)
                    .Append(".").Append(filterContext.ActionDescriptor.ActionName);
                if (filterContext.ActionParameters.Count > 0)
                {
                    message.Append("(");
                    var count = 0;
                    foreach (var parameter in filterContext.ActionParameters)
                    {
                        message.Append(parameter.Key).Append(":").Append(parameter.Value);
                        count++;
                        if (count < filterContext.ActionParameters.Count)
                        {
                            message.Append(", ");
                        }
                    }
                    message.Append(")");
                }

                log.Info(message);
            }
        }
    }
}