using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CheckArgentina.Startup))]
namespace CheckArgentina
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
