using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WCFContract;

namespace WebAPIWCFClientAsMVC.Controllers
{
    public class HomeController : Controller
    {
        private ICalculator _calculator;
        public HomeController(ICalculator calculator)
        {
            _calculator = calculator;
        }
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var x = _calculator.Add(1, 3);

            return View();
        }

        // Cross origin resource sharing
        public ActionResult CORS()
        {
           
            return View();
        }


    }
}
