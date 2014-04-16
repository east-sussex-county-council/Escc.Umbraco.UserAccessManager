using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using UmbracoUserControl.Services;

namespace UmbracoUserControl.Installers
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