
using System.Web.Mvc;

namespace STSServerAndService.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MetaData()
        {

            return null;
            // return Content(xmlString, "text/xml");

        }

     
    }
}