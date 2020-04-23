using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Http;
using System.Configuration;
using System.Web.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;

using Microsoft.EntityFrameworkCore;

using TextPortCore.Data;

[assembly: OwinStartupAttribute(typeof(TextPort.Startup))]
namespace TextPort
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var services = new ServiceCollection();

            ConfigureAuth(app);

            ConfigureServices(services);
            var resolver = new DefaultDependencyResolver(services.BuildServiceProvider());
            DependencyResolver.SetResolver(resolver); //Set MVC
            GlobalConfiguration.Configuration.DependencyResolver = resolver; //Set for Web API

            app.MapSignalR();

            GlobalVariables.Environment = ConfigurationManager.AppSettings["Environment"];
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TextPortContext>(options => options.UseSqlServer(WebConfigurationManager.ConnectionStrings["TextPortContext"].ConnectionString));

            services.AddControllersAsServices(typeof(Startup).Assembly.GetExportedTypes()
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition).Where(t => typeof(IController).IsAssignableFrom(t) || typeof(System.Web.Http.ApiController).IsAssignableFrom(t)));
        }
    }
}
