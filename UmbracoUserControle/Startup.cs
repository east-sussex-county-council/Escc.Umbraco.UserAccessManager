using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UmbracoUserControl.Startup))]
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
