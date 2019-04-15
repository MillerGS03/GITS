using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GITS.Controllers
{
    public class MainController : Controller
    {
        // GET: Main
        public ActionResult Login()
        {           
            return Request.Cookies["user"] != null ? View("Index") : View("Login");
        }

        public ActionResult Index()
        {
            return Request.Cookies["user"] != null ? View("Index") : View("Login");
        }
        public ActionResult Calendario()
        {
            return PartialView();
        }
    }
}