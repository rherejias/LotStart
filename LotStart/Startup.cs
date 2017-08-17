using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LotStart.Startup))]
namespace LotStart
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
