using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DoTask.Startup))]
namespace DoTask
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
