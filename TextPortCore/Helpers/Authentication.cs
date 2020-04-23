using System;
using System.Security.Principal;
using System.Security.Claims;
using System.Web;

namespace TextPortCore.Helpers
{
    public static class Authentication
    {
        public static bool IsUserLoggedIn(IPrincipal user)
        {
            if (user != null)
            {
                if (user.Identity.IsAuthenticated)
                {
                    if (ClaimsPrincipal.Current != null)
                    {
                        return ClaimsPrincipal.Current.FindFirst("AccountId").Value != "-99";
                    }
                }
            }
            return false;
        }
    }
}
