using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
//using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;

using TextPortCore.Models;
using TextPortCore.Data;

namespace TextPortAPI.Authorization
{
    public class BasicAuthHttpModule : IHttpModule
    {
        //const String AUTH_USER_PREFIX = "auth.user.";
        //static readonly IDictionary<String, String> logins = ConfigurationManager.AppSettings.AllKeys.Where(k => k.StartsWith(AUTH_USER_PREFIX))
        //.ToDictionary(key => key.Replace(AUTH_USER_PREFIX, String.Empty), value => ConfigurationManager.AppSettings.Get(value));
        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += OnAuthenticateRequest;
            context.EndRequest += OnEndRequest;
        }

        private static void OnAuthenticateRequest(object sender, EventArgs e)
        {
            var authHeaders = HttpContext.Current.Request.Headers["Authorization"];
            if (authHeaders != null)
            {
                var authHeadersValue = AuthenticationHeaderValue.Parse(authHeaders);
                if (authHeadersValue.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) && !String.IsNullOrWhiteSpace(authHeadersValue.Parameter))
                {
                    try
                    {
                        var credentials = authHeadersValue.Parameter;
                        var encoding = Encoding.GetEncoding("iso-8859-1");
                        credentials = encoding.GetString(Convert.FromBase64String(credentials));
                        string token = credentials.Split(':').First();
                        string secret = credentials.Split(':').Last();

                        using (TextPortDA da = new TextPortDA())
                        {
                            APIApplication apiApp = da.AuthorizeAPI(token, secret);
                            if (apiApp != null)
                            {
                                //Set the principal for validated user
                                List<Claim> claims = new List<Claim> {
                                    new Claim(ClaimTypes.Name, token),
                                    new Claim("AccountId", apiApp.AccountId.ToString(), ClaimValueTypes.Integer),
                                    new Claim("ApiApplicationId", apiApp.APIApplicationId.ToString(), ClaimValueTypes.Integer)
                                };

                                ClaimsIdentity identity = new ClaimsIdentity(claims, "ApplicationCookie");
                                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                                //var principal = new GenericPrincipal(new GenericIdentity(apiApp.AccountId.ToString()), null);
                                Thread.CurrentPrincipal = principal;
                                if (HttpContext.Current != null)
                                {
                                    HttpContext.Current.User = principal;
                                }
                            }
                            else
                            {
                                //Authentication failed
                                HttpContext.Current.Response.StatusCode = 401;
                            }
                        }
                    }
                    catch (FormatException)
                    {
                        HttpContext.Current.Response.StatusCode = 401;
                    }
                }
            }
        }

        private static void OnEndRequest(object sender, EventArgs e)
        {
            var response = HttpContext.Current.Response;
            if (response.StatusCode == 401)
            {
                //Addh eaders if authentication failed
                response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", ConfigurationManager.AppSettings.Get("auth.realm")));
            }
        }

        public void Dispose() { }

    }
}
