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
                    ViewBag.Usuario = new Usuario((int)new JavaScriptSerializer().Deserialize(Request.Cookies["user"].Value.Substring(6), typeof(int)));
                    if (ViewBag.Usuario.Tarefas != null)
                        return View("Index");
                    else
                        throw new Exception();
                }
                catch {
                    try { Index(); }
                    catch { return null; }
                }
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
            try
            {
                string idUrl = (string)RouteData.Values["id"];

                if (idUrl == null)
                {
                    var cookie = Request.Cookies["user"];
                    if (cookie != null)
                    {
                        var cookieUsuario = cookie.Value.Substring(6);
                        var json = new JavaScriptSerializer();
                        ViewBag.Usuario = new Usuario((int)json.Deserialize(cookieUsuario, typeof(int)));
                    }
                }
                else if (int.TryParse(idUrl, out int id))
                    ViewBag.Usuario = new Usuario(id);
                return View();
            }
            catch(Exception e) { return null; }
        }
        public ActionResult _Calendario()
        {
            return PartialView();
        }
        public ActionResult Sobre()
        {
            return View();
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
            cookie.Values.Add("login", new JavaScriptSerializer().Serialize(loginInfo.Id));
            cookie.Expires = DateTime.Now.AddDays(15);
            cookie.HttpOnly = false;
            Response.AppendCookie(cookie);
            return RedirectToAction("index");

        }
        public string GetUsuario(int id)
        {
            if ((int)new JavaScriptSerializer().Deserialize(Request.Cookies["user"].Value.Substring(6), typeof(int)) == id)
                return new JavaScriptSerializer().Serialize(new Usuario(id));
            return "login=0";
        }


        // METODOS POST


        [HttpPost]
        public ActionResult EnviarSolicitacaoPara(int idUsuario)
        {
            try
            {
                int atual = (int)new JavaScriptSerializer().Deserialize(Request.Cookies["user"].Value.Substring(6), typeof(int));
                Dao.Usuarios.CriarAmizade(atual, idUsuario);
                return Json("Sucesso");
            }
            catch (Exception e) { return Json(e.Message); }
        }
        [HttpPost]
        public ActionResult CriarTarefa(Tarefa evento, string nomeMeta, string[] convites)
        {
            try
            {
                Array.Sort(convites, StringComparer.InvariantCulture);
                Usuario atual = new Usuario((int)new JavaScriptSerializer().Deserialize(Request.Cookies["user"].Value.Substring(6), typeof(int)));
                atual.Amigos = atual.Amigos.OrderBy(o => o.Nome).ToList();
                evento.CodUsuarioCriador = atual.Id;
                if (nomeMeta != null && nomeMeta.Trim() != "")
                    evento.Meta = Dao.Eventos.Metas(evento.CodUsuarioCriador).Find(m => m.Titulo == nomeMeta);
                Dao.Eventos.CriarTarefa(ref evento);
                int indexConvites = 0;
                foreach(Amigo a in atual.Amigos)
                {
                    if (a.Nome.Contains(convites[indexConvites]))
                    {
                        Dao.Eventos.AdicionarUsuarioATarefa(a.Id, evento.CodTarefa);
                        indexConvites++;
                    }
                }
                return Json("Sucesso");
            }
            catch (Exception e) { return Json(e.Message); }
        }
        [HttpPost]
        public ActionResult CriarAcontecimento(Acontecimento evento)
        {
            try
            {
                Usuario criador = new Usuario((int)new JavaScriptSerializer().Deserialize(Request.Cookies["user"].Value.Substring(6), typeof(int)));
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
                if (tipo == 1)
                    Dao.Eventos.AdicionarUsuarioATarefa((int)new JavaScriptSerializer().Deserialize(Request.Cookies["user"].Value.Substring(6), typeof(int)), cod);
                else if (tipo == 2)
                    Dao.Eventos.AdicionarUsuarioAAcontecimento((int)new JavaScriptSerializer().Deserialize(Request.Cookies["user"].Value.Substring(6), typeof(int)), cod);
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
                atual = new Usuario((int)new JavaScriptSerializer().Deserialize(Request.Cookies["user"].Value.Substring(6), typeof(int)));
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
                HttpCookie cookie = new HttpCookie("user");
                cookie.Values.Add("login", new JavaScriptSerializer().Serialize(atual.Id));
                cookie.Expires = DateTime.Now.AddDays(15);
                cookie.HttpOnly = false;
                Response.AppendCookie(cookie);
                return Json("Sucesso");
            }
            catch (Exception ex) { return Json(ex.Message); }
        }
        [HttpPost]
        public ActionResult AtualizarXP(int xp)
        {
            Usuario atual;
            try
            {
                atual = new Usuario((int)new JavaScriptSerializer().Deserialize(Request.Cookies["user"].Value.Substring(6), typeof(int)));
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
        public ActionResult RemoverAmizade(int idUsuario)
        {
            Usuario atual;
            try
            {
                atual = new Usuario((int)new JavaScriptSerializer().Deserialize(Request.Cookies["user"].Value.Substring(6), typeof(int)));
                if (atual == null)
                    throw new Exception();
            }
            catch { throw new Exception("Usuário não encontrado. Faça login para editar o status!"); }
            try
            {
                Dao.Usuarios.RemoverAmizade(atual.Id, idUsuario);
                return Json("Sucesso");
            }
            catch (Exception ex) { return Json(ex.Message); }
        }
    }
}