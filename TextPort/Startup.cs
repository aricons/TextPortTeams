using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Http;
using System.Web.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;

using Microsoft.EntityFrameworkCore;

using TextPortCore.Data;
using TextPortCore.Helpers.CustomValidation;

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

            // DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RequiredIfAttribute), typeof(RequiredAttributeAdapter));

            app.MapSignalR();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TextPortContext>(options => options.UseSqlServer(WebConfigurationManager.ConnectionStrings["TextPortContext"].ConnectionString));

            //services.AddControllersAsServices(typeof(Startup).Assembly.GetExportedTypes()
            //   .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
            //   .Where(t => typeof(IController).IsAssignableFrom(t) || typeof(IHttpController).IsAssignableFrom(t))); // || t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)));

            services.AddControllersAsServices(typeof(Startup).Assembly.GetExportedTypes()
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition).Where(t => typeof(IController).IsAssignableFrom(t) || typeof(System.Web.Http.ApiController).IsAssignableFrom(t)));
        }
    }
}
