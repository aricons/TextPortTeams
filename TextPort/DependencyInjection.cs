using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace TextPort
{
    public class DefaultDependencyResolver :
    System.Web.Http.Dependencies.IDependencyResolver,
    System.Web.Mvc.IDependencyResolver
    {

        private readonly IServiceProvider serviceProvider;

        public DefaultDependencyResolver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public object GetService(Type serviceType)
        {
            return this.serviceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.serviceProvider.GetServices(serviceType);
        }

        //Web API specific

        public System.Web.Http.Dependencies.IDependencyScope BeginScope()
        {
            return this;
        }

        public void Dispose()
        {
            // NO-OP, as the container is shared. 
        }
    }

    //public class DefaultDependencyResolver : IDependencyResolver, System.Web.Http.Dependencies.IDependencyResolver
    //{
    //    protected IServiceProvider serviceProvider;

    //    public DefaultDependencyResolver(IServiceProvider serviceProvider)
    //    {
    //        this.serviceProvider = serviceProvider;
    //    }

    //    public object GetService(Type serviceType)
    //    {
    //        return this.serviceProvider.GetService(serviceType);
    //    }

    //    public IEnumerable<object> GetServices(Type serviceType)
    //    {
    //        return this.serviceProvider.GetServices(serviceType);
    //    }

    //    // For WebApi Controllers
    //    public System.Web.Http.Dependencies.IDependencyScope BeginScope()
    //    {
    //        return this;
    //    }
    //    public void Dispose()
    //    {
    //        // NO-OP, as the container is shared. 
    //    }
    //}

    public static class ServiceProviderExtensions
    {
        public static IServiceCollection AddControllersAsServices(this IServiceCollection services,
      IEnumerable<Type> controllerTypes)
        {
            foreach (var type in controllerTypes)
            {
                services.AddTransient(type);
            }

            return services;
        }
        //public static IServiceCollection AddControllersAsServices(this IServiceCollection services,
        //   IEnumerable<Type> controllerTypes)
        //{
        //    foreach (var type in controllerTypes)
        //    {
        //        services.AddTransient(type);
        //    }

        //    return services;
        //}
        //}
    }
}