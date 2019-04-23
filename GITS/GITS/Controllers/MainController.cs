using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GITS.ViewModel;
using Microsoft.Owin.Security.Cookies;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.Web.Script.Serialization;

namespace GITS.Controllers
{
    public class MainController : Controller
    {
        // GET: Main
        public ActionResult Login()
        {
            if (Request.Cookies["user"] == null)
                return View("Login");
            ConfigurarUsuario();
            return RedirectToAction("index");
        }
        public ActionResult Index()
        {
            if (Request.Cookies["user"] != null)
            {
                ConfigurarUsuario();
                return View("Index");
            }
            return RedirectToAction("login");
        }
        public ActionResult Bastidores()
        {
            return View();
        }
        public ActionResult FAQ()
        {
            return View();
        }
        public ActionResult _Calendario()
        {
            return PartialView();
        }
        public void SignIn(string ReturnUrl = "/", string type = "")
        {
            if (!Request.IsAuthenticated)
            {
                if (type == "Google")
                {
                    HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/LoginWithGoogle" }, "Google");
                }
            }
        }
        public ActionResult SignOut()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Redirect("~/");
        }

        public ActionResult LoginWithGoogle()
        {
            var claimsPrincipal = HttpContext.User.Identity as ClaimsIdentity;

            var loginInfo = Usuario.GetLoginInfo(claimsPrincipal);
            if (loginInfo == null)
            {
                return RedirectToAction("Index");
            }

            HttpCookie cookie = new HttpCookie("user");
            cookie.Values.Add("login", new JavaScriptSerializer().Serialize(loginInfo));
            cookie.Expires = DateTime.Now.AddDays(15);
            cookie.HttpOnly = false;
            Response.AppendCookie(cookie);
            return RedirectToAction("index");

        }
        public void ConfigurarUsuario()
        {
            Usuario atual = (Usuario)new JavaScriptSerializer().Deserialize(Request.Cookies["user"].Value.Substring(6), typeof(Usuario));
            Usuario bd = new Dao().Usuarios.ToList().Find(u => u.Id == atual.Id);
            if (atual != null && !atual.Equals(bd))
            {
                HttpCookie cookie = new HttpCookie("user");
                cookie.Values.Add("login", new JavaScriptSerializer().Serialize(bd));
                cookie.Expires = DateTime.Now.AddDays(15);
                cookie.HttpOnly = false;
                Response.AppendCookie(cookie);
            }
        }
    }
}