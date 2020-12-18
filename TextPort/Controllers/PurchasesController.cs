using System.Web.Mvc;
using System.Security.Claims;

using TextPortCore.Models;
using TextPortCore.Helpers;

namespace TextPort.Controllers
{
    public class PurchasesController : Controller
    {
        public ActionResult Index()
        {
            int accountId = Utilities.GetAccountIdFromClaim(ClaimsPrincipal.Current);

            PurchasesContainer purchasesContainer = new PurchasesContainer(accountId);

            return View(purchasesContainer);
        }
    }
}