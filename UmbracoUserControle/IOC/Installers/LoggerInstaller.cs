using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using log4net.Config;

namespace UmbracoUserControl.IOC.Installers
{
    public class LoggerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //container.AddFacility<LoggingFacility>(f => f.UseLog4Net("log4net.config"));
            container.AddFacility<LoggingFacility>(f => f.UseLog4Net());
            //container.AddFacility<LoggingFacility>(f => f.UseLog4Net());
            XmlConfigurator.Configure();
        }
    }
}