using Microsoft.Owin;
using Owin;
using UmbracoUserControl;

[assembly: OwinStartup(typeof(Startup))]

namespace UmbracoUserControl
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}