using System.Web.Mvc;

namespace TheGapFillers.Portrack.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
