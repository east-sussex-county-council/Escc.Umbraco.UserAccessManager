using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using ESCC.Umbraco.UserAccessManager.Services;
using ESCC.Umbraco.UserAccessManager.Services.Interfaces;

namespace ESCC.Umbraco.UserAccessManager.IOC.Installers
{
    public class EmailServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IEmailService>()
                                        .ImplementedBy<EmailService>()
                                        .LifestyleTransient());
        }
    }
}