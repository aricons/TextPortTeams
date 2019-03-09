using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System.IO;


//[assembly: OwinStartup(typeof(TextPort.OWINStartup))]

namespace TextPort
{
    public class OWINStartup
    {
        public void Configuration(IAppBuilder app)
        {
            //var cookieAuthOptions = new CookieAuthenticationOptions
            //{
            //    AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
            //    CookieHttpOnly = true,
            //    ExpireTimeSpan = TimeSpan.FromMinutes(60),
            //    SlidingExpiration = true,
            //    CookieSecure = CookieSecureOption.SameAsRequest,
            //    LoginPath = new PathString("/Account/Login")
            //};
            //app.UseCookieAuthentication(cookieAuthOptions);

            //app.Use((context, next) =>
            //{
            //SetCookieAuthenticationAsDefault(app);

            //TextWriter output = context.Get<TextWriter>("host.TraceOutput");
            //return next().ContinueWith(result =>
            //{
            //    output.WriteLine("Scheme {0} : Method {1} : Path {2} : MS {3}",
            //    context.Request.Scheme, context.Request.Method, context.Request.Path, getTime());
            //});
            //});

            //app.Run(async context =>
            //{
            //    await context.Response.WriteAsync(getTime() + " TextPort OWIN startup");
            //});
        }

        private void SetCookieAuthenticationAsDefault(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
        }

        //string getTime()
        //{
        //    return DateTime.Now.Millisecond.ToString();
        //}
    }
}
