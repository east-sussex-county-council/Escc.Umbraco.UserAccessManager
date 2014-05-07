using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UmbracoUserControl.Services;
using UmbracoUserControl.Services.Interfaces;

namespace UmbracoUserControl.IOC.Installers
{
    public class PermissionsServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IPermissionsControlService>()
                                        .ImplementedBy<PermissionsControlService>()
                                        .LifestyleTransient());
        }
    }
}