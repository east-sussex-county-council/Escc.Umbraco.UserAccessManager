using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using UmbracoUserControl.Services;
using UmbracoUserControl.Services.Interfaces;

namespace UmbracoUserControl.IOC.Installers
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