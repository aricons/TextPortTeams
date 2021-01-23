using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextPortTeams.Helpers
{
    public static class Cookies
    {
        public static void Write(string cookieName, string value, double expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie == null)
            {
                cookie = new HttpCookie(cookieName);
                if (expires > 0) // Only set expiration date if > 0, otherwise the cookie is good until the browser is closed only.
                {
                    cookie.Expires = DateTime.Now.AddDays(expires);
                }
                cookie.Value = value;
            }
            else
            {
                cookie.Value = value;
            }

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static string Read(string cookieName)
        {
            string cookieValue = string.Empty;

            if (HttpContext.Current.Request.Cookies[cookieName] != null)
            {
                cookieValue = HttpContext.Current.Request.Cookies[cookieName].Value;
            }
            return cookieValue;
        }

        public static decimal ReadAsDecimal(string cookieName)
        {
            decimal cookieValue = 0;

            if (HttpContext.Current.Request.Cookies[cookieName] != null)
            {
                decimal.TryParse(HttpContext.Current.Request.Cookies[cookieName].Value, out cookieValue);
            }
            return cookieValue;
        }

        public static void ClearCookie(string cookieName)
        {
            if (HttpContext.Current.Request.Cookies[cookieName] != null)
            {
                HttpCookie cookie = new HttpCookie(cookieName);
                cookie.Expires = DateTime.Now.AddDays(-1d);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        public static void WriteBalance(decimal balance)
        {
            Write("balance", balance.ToString(), 0);
        }

        public static string GetBalance()
        {
            return Read("balance");
        }
    }
}