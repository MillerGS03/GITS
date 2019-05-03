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
using static GITS.ViewModel.Usuario;

namespace GITS.Controllers
{
    public class MainController : Controller
    {
        // METODOS GET
        public ActionResult Login()
        {
            if (Request.Cookies["user"] == null)
                return View("Login");

            try
            {
                ConfigurarUsuario();
            }
            catch // Possivelmente cookie inválido
            {
                Response.Cookies.Remove("user");
                return View("Login");
            }
            return RedirectToAction("index");
        }
        public ActionResult Index()
        {
            if (Request.Cookies["user"] != null)
            {
                try
                {
                    ConfigurarUsuario();
                }
                catch { Response.Cookies.Remove("user"); return RedirectToAction("Login"); }
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
        public ActionResult Perfil()
        {
            ConfigurarUsuario();

            string idUrl = (string)RouteData.Values["id"];

            if (idUrl == null)
            {
                var cookie = Request.Cookies["user"];
                if (cookie != null)
                {
                    var cookieUsuario = cookie.Value.Substring(6);
                    var json = new JavaScriptSerializer();
                    ViewBag.Usuario = (Usuario)json.Deserialize(cookieUsuario, typeof(Usuario));
                }
            }
            else if (int.TryParse(idUrl, out int id))
            {
                ViewBag.Usuario = Dao.Usuarios.ToList().Find(usuario => usuario.Id == id);
            }

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
            Response.Cookies["user"].Expires = DateTime.UtcNow;
            return RedirectToAction("Index");
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
            Usuario bd = Dao.Usuarios.ToList().Find(u => u.Id == atual.Id);
            if (atual != null && !atual.Equals(bd))
            {
                HttpCookie cookie = new HttpCookie("user");
                cookie.Values.Add("login", new JavaScriptSerializer().Serialize(bd));
                cookie.Expires = DateTime.Now.AddDays(15);
                cookie.HttpOnly = false;
                Response.AppendCookie(cookie);
            }
        }


        // METODOS POST


        [HttpPost]
        public ActionResult EnviarSolicitacaoPara(int idUsuario)
        {
            try
            {
                int atual = ((Usuario)new JavaScriptSerializer().Deserialize(Request.Cookies["user"].Value.Substring(6), typeof(Usuario))).Id;
                Dao.Usuarios.CriarAmizade(atual, idUsuario);
                return Json("Sucesso");
            }
            catch (Exception e) { return Json(e.Message); }
        }
        [HttpPost]
        public ActionResult CriarTarefa(Tarefa evento)
        {
            try
            {
                Usuario criador = (Usuario)new JavaScriptSerializer().Deserialize(Request.Cookies["user"].Value.Substring(6), typeof(Usuario));
                Dao.Eventos.CriarTarefa(evento);
                return Json("Sucesso");
            }
            catch (Exception e) { return Json(e.Message); }
        }
        [HttpPost]
        public ActionResult CriarAcontecimento(Acontecimento evento)
        {
            try
            {
                Usuario criador = (Usuario)new JavaScriptSerializer().Deserialize(Request.Cookies["user"].Value.Substring(6), typeof(Usuario));
                Dao.Eventos.CriarAcontecimento(evento);
                return Json("Sucesso");
            }
            catch (Exception e) { return Json(e.Message); }
        }
        [HttpPost]
        public ActionResult AdicionarUsuarioA(int cod, int tipo)
        {
            try
            {
                Usuario atual = (Usuario)new JavaScriptSerializer().Deserialize(Request.Cookies["user"].Value.Substring(6), typeof(Usuario));
                if (tipo == 1)
                    Dao.Eventos.AdicionarUsuarioATarefa(atual, cod);
                else if (tipo == 2)
                    Dao.Eventos.AdicionarUsuarioAAcontecimento(atual, cod);
                return Json("Sucesso");
            }
            catch (Exception e) { return Json(e.Message); }
        }

        [HttpPost]
        public ActionResult AtualizarStatus(string status)
        {
            Usuario atual;
            try
            {
                atual = (Usuario)new JavaScriptSerializer().Deserialize(Request.Cookies["user"].Value.Substring(6), typeof(Usuario));
                if (atual == null)
                    throw new Exception();
            }
            catch { throw new Exception("Usuário não encontrado. Faça login para editar o status!"); }

            if (status.Trim().Length > 50)
                throw new Exception("O status deve ter no máximo 50 caracteres!");
            atual.Status = status;

            try
            {
                Dao.Usuarios.Update(atual);
                return Json("Sucesso");
            }
            catch (Exception ex) { return Json(ex.Message); }
        }
        public ActionResult AtualizarXP(int xp)
        {
            Usuario atual;
            try
            {
                atual = (Usuario)new JavaScriptSerializer().Deserialize(Request.Cookies["user"].Value.Substring(6), typeof(Usuario));
                if (atual == null)
                    throw new Exception();
            }
            catch { throw new Exception("Usuário não encontrado. Faça login para editar o status!"); }
            atual.XP += xp;
            try
            {
                Dao.Usuarios.Update(atual);
                return Json("Sucesso");
            }
            catch (Exception ex) { return Json(ex.Message); }
        }
    }
}