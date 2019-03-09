using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Compilation;
using System.Web.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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
           
            //var resolver = new DefaultDependencyResolver(services.BuildServiceProvider());
            //DependencyResolver.SetResolver(resolver); // Set for MVC pages
            //GlobalConfiguration.Configuration.DependencyResolver = resolver; //Set for WebAPI controllers

            // Test
            ConfigureServices(services);
            var resolver = new DefaultDependencyResolver(services.BuildServiceProvider());
            DependencyResolver.SetResolver(resolver);//Set MVC
            GlobalConfiguration.Configuration.DependencyResolver = resolver; //Set for Web API

            app.MapSignalR();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //..services.AddSqlServer();
            //var connectionString = System.Web.Compilation GetConnectionString("myDb");
            //services.AddDbContext<TextPortContext>(options => options.UseSqlServer("")

            //string connString = WebConfigurationManager.ConnectionStrings["MyConnectionStringNameHere"].ConnectionString;

            services.AddDbContext<TextPortContext>(options => options.UseSqlServer(WebConfigurationManager.ConnectionStrings["TextPortContext"].ConnectionString));

            // Register the service and implementation for the database context
            //services.AddScoped<TextPortContext>(provider => provider.GetService<TextPortContext>());

            //services.AddControllersAsServices(typeof(Startup).Assembly.GetExportedTypes()
            //   .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
            //   .Where(t => typeof(IController).IsAssignableFrom(t) || typeof(IHttpController).IsAssignableFrom(t))); // || t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)));

            services.AddControllersAsServices(typeof(Startup).Assembly.GetExportedTypes()
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition).Where(t => typeof(IController).IsAssignableFrom(t) || typeof(System.Web.Http.ApiController).IsAssignableFrom(t)));
        }
    }
}
