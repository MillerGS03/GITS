using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GITS.Controllers
{
    public class MainController : Controller
    {
        bool logado = false;
        // GET: Main
        public ActionResult Index()
        {
            return Request.Cookies["user"] != null ? View("Principal") : View("Index");
        }

        public ActionResult Principal()
        {
            logado = true;
            return View();
        }
    }
}