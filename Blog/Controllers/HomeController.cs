using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Welcome to my blog about all of the coolest things";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact me yo!";

            return View();
        }
    }
}
