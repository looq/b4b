using System.Web.Mvc;

namespace Breast_For_Baby.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/
        public ActionResult Oops()
        {
            return View();
        }
	}
}