using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco7._0._0.Services;
using UmbracoUserControl.Services;

namespace UmbracoUserControl.Plumbing.Installers
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