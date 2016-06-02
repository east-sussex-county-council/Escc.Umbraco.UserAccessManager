using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Escc.Umbraco.UserAccessManager.Services;
using Escc.Umbraco.UserAccessManager.Services.Interfaces;

namespace Escc.Umbraco.UserAccessManager.IOC.Installers
{
    public class DatabaseInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IDatabaseService>()
                                        .ImplementedBy<DatabaseService>()
                                        .LifestyleTransient());
        }
    }
}