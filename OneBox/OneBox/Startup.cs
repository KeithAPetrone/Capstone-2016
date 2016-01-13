using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OneBox.Startup))]
namespace OneBox
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
