using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Coffee0417.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            Response.Redirect("vacationaroma/index.html");
            return View();
        }
    }
}