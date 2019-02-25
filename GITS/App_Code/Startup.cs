using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GITS.Startup))]
namespace GITS
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
