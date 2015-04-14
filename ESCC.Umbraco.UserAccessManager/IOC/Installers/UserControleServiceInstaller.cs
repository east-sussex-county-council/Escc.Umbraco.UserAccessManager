using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using ESCC.Umbraco.UserAccessManager.Services;
using ESCC.Umbraco.UserAccessManager.Services.Interfaces;

namespace ESCC.Umbraco.UserAccessManager.IOC.Installers
{
    public class UserControleServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IUserControlService>()
                                        .ImplementedBy<UserControlService>()
                                        .LifestyleTransient());
        }
    }
}