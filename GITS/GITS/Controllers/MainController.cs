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
            
            return RedirectToAction("index");
        }
        public ActionResult Index()
        {
            if (Request.Cookies["user"] != null)
                return View("Index");

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


            //   WebEntities db = new WebEntities(); //DbContext
            //   var user = db.UserAccounts.FirstOrDefault(x => x.Email == loginInfo.emailaddress);

            //   if (user == null)
            //   {
            //       user = new UserAccount
            //       {
            //           Email = loginInfo.emailaddress,
            //           GivenName = loginInfo.givenname,
            //           Identifier = loginInfo.nameidentifier,
            //           Name = loginInfo.name,
            //           SurName = loginInfo.surname,
            //           IsActive = true
            //       };
            //       db.UserAccounts.Add(user);
            //       db.SaveChanges();
            //   }

            //   var ident = new ClaimsIdentity(
            //           new[] { 
            //// adding following 2 claim just for supporting default antiforgery provider
            //new Claim(ClaimTypes.NameIdentifier, user.Email),
            //                           new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

            //                           new Claim(ClaimTypes.Name, user.Name),
            //                           new Claim(ClaimTypes.Email, user.Email),
            //// optionally you could add roles if any
            //new Claim(ClaimTypes.Role, "User")
            //           },
            //           CookieAuthenticationDefaults.AuthenticationType);


            //HttpContext.GetOwinContext().Authentication.SignIn(
            //            new AuthenticationProperties { IsPersistent = false }, ident);
            return View();

        }
    }
}