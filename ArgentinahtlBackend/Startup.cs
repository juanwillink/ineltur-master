using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ArgentinahtlBackend.Startup))]
namespace ArgentinahtlBackend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
