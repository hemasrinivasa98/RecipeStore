using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RecipeStore.Startup))]
namespace RecipeStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
