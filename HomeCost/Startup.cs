using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HomeCost.Startup))]
namespace HomeCost
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
