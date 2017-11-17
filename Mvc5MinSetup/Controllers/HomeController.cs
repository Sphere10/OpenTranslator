using System.Web.Mvc;

namespace Mvc5MinSetup.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}