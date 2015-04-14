using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;
using Castle.Windsor.Installer;
using ESCC.Umbraco.UserAccessManager.IOC;

namespace ESCC.Umbraco.UserAccessManager
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            BootstrapContainer();
        }

        private static IWindsorContainer _container;

        private static void BootstrapContainer()
        {
            _container = new WindsorContainer()
                    .Install(FromAssembly.This());

            var controllerFactory = new WindsorControllerFactory(_container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }

        protected void Application_End()
        {
            _container.Dispose();
        }
    }
}