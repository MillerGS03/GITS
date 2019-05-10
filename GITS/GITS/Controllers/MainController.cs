﻿using System;
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

        const string senhaCriptografia = "Lorem ipsum batatae";

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
                    ViewBag.Usuario = new Usuario(GetId());
                    if (ViewBag.Usuario.Tarefas != null)
                    {
                        ViewBag.Feed = Dao.Usuarios.PublicacoesRelacionadasA(ViewBag.Usuario.Id);
                        return View("Index");
                    }
                    else
                        throw new Exception();
                }
                catch
                {
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
        private int GetId()
        {
            if (Request.Cookies["user"] != null)
                try
                {
                    var idCriptografado = (string)new JavaScriptSerializer().Deserialize(Request.Cookies["user"].Value.Substring(6), typeof(string));
                    return int.Parse(StringCipher.Decrypt(idCriptografado, senhaCriptografia));
                }
                catch
                {
                    throw new Exception("Cookie inválido");
                }
            throw new Exception("Cookie não encontrado");

        }
        public ActionResult Perfil()
        {
            try
            {
                ViewBag.IsYourself = false;
                ViewBag.IsYourFriend = false;
                ViewBag.IsLoggedIn = false;

                Usuario usuarioLogado = null;

                if (Request.Cookies["user"] != null)
                {
                    usuarioLogado = new Usuario(GetId());
                    ViewBag.IsLoggedIn = true;
                }

                string idUrl = (string)RouteData.Values["id"];

                if (idUrl == null && usuarioLogado != null)
                {
                    ViewBag.Usuario = usuarioLogado;
                    ViewBag.Publicacoes = Dao.Usuarios.Publicacoes(usuarioLogado.Id);
                    ViewBag.IsYourself = true;
                }
                else if (int.TryParse(idUrl, out int id))
                {
                    var usuario = new Usuario(id);
                    if (usuario.Id == 0)
                        ViewBag.Usuario = null;
                    else
                    {
                        ViewBag.Usuario = usuario;
                        if (usuario.Equals(usuarioLogado))
                            ViewBag.IsYourself = true;
                        ViewBag.Publicacoes = Dao.Usuarios.Publicacoes(id);

                        if (usuario != null && usuarioLogado != null)
                        {
                            foreach (var amigo in usuario.Amigos)
                                if (amigo.Id == usuarioLogado.Id)
                                {
                                    ViewBag.IsYourFriend = true;
                                    break;
                                }
                        }
                    }
                }

                return View();
            }
            catch { return null; }
        }
        public ActionResult _Calendario()
        {
            return PartialView();
        }
        public ActionResult Sobre()
        {
            return View();
        }
        public ActionResult Publicacao()
        {
            string idUrl = (string)RouteData.Values["id"];
            if (idUrl == null || idUrl == "")
                return RedirectToAction("Index", "Main");

            ViewBag.IsYourself = false;
            if (int.TryParse(idUrl, out int id))
            {
                ViewBag.Publicacao = Dao.Usuarios.Publicacao(id);
                if (ViewBag.Publicacao.IdPublicacao != 0)
                {
                    ViewBag.UsuarioCriador = Dao.Usuarios.GetUsuario(ViewBag.Publicacao.IdUsuario);
                    try
                    {
                        ViewBag.IsYourself = ViewBag.Publicacao.IdUsuario == GetId();
                    }
                    catch { ViewBag.IsYourself = false; }
                }
                else
                    ViewBag.Publicacao = null;
            }
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

            var idCriptografado = StringCipher.Encrypt(loginInfo.Id.ToString(), senhaCriptografia);
            HttpCookie cookie = new HttpCookie("user");
            cookie.Values.Add("login", new JavaScriptSerializer().Serialize(idCriptografado));
            cookie.Expires = DateTime.Now.AddDays(15);
            cookie.HttpOnly = false;
            Response.AppendCookie(cookie);
            return RedirectToAction("index");

        }
        public string GetUsuario(string id)
        {
            var idLogado = GetId();
            var idParametro = int.Parse(StringCipher.Decrypt(id, senhaCriptografia));
            if (idLogado == idParametro)
                return new JavaScriptSerializer().Serialize(new Usuario(idLogado));
            return "login=0";
        }


        // METODOS POST


        [HttpPost]
        public ActionResult EnviarSolicitacaoPara(int idUsuario)
        {
            try
            {
                Dao.Usuarios.CriarAmizade(GetId(), idUsuario);
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
                Usuario atual = new Usuario(GetId());
                atual.Amigos = atual.Amigos.OrderBy(o => o.Nome).ToList();
                evento.CodUsuarioCriador = atual.Id;
                if (nomeMeta != null && nomeMeta.Trim() != "")
                    evento.Meta = Dao.Eventos.Metas(evento.CodUsuarioCriador).Find(m => m.Titulo == nomeMeta);
                Dao.Eventos.CriarTarefa(ref evento);
                int indexConvites = 0;
                foreach (Amigo a in atual.Amigos)
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
                Usuario criador = new Usuario(GetId());
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
                    Dao.Eventos.AdicionarUsuarioATarefa(GetId(), cod);
                else if (tipo == 2)
                    Dao.Eventos.AdicionarUsuarioAAcontecimento(GetId(), cod);
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
                atual = new Usuario(GetId());
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
                cookie.Values.Add("login", new JavaScriptSerializer().Serialize(StringCipher.Encrypt(atual.Id.ToString(), senhaCriptografia)));
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
                atual = new Usuario(GetId());
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
        [HttpPost]
        public ActionResult RemoverAmizade(int idUsuario)
        {
            Usuario atual;
            try
            {
                atual = new Usuario(GetId());
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
        [HttpPost]
        public string AceitarSolicitacaoDeAmizade(int idNotificacao, int codAmizade, Notificacao n)
        {
            if (!UsuarioLogado())
                throw new Exception("Usuário não encontrado. Faça login para aceitar solicitação!");
            try
            {
                Dao.Usuarios.AceitarAmizade(codAmizade, n);
                Dao.Usuarios.VisualizarNotificacao(idNotificacao);
                return new JavaScriptSerializer().Serialize(new Usuario(GetId()));
            }
            catch (Exception e) { throw e; }
        }
        [HttpPost]
        public string RecusarSolicitacaoDeAmizade(int idNotificacao, int codAmizade)
        {
            if (!UsuarioLogado())
                throw new Exception("Usuário não encontrado. Faça login para recusar solicitação!");
            Dao.Usuarios.RecusarAmizade(codAmizade);
            Dao.Usuarios.VisualizarNotificacao(idNotificacao);
            return new JavaScriptSerializer().Serialize(new Usuario(GetId()));
        }
        [HttpPost]
        public ActionResult Publicar(string titulo, string descricao, int[] idsUsuariosMarcados)
        {
            Usuario atual;
            try
            {
                atual = new Usuario(GetId());
                if (atual == null || atual.Id == 0)
                    throw new Exception();
            }
            catch { throw new Exception("Usuário não encontrado. Faça login para publicar!"); }
            try
            {
                Dao.Usuarios.Publicar(new Publicacao(atual.Id, titulo, descricao, DateTime.Now), idsUsuariosMarcados);
                return Json("Sucesso");
            }
            catch (Exception ex) { return Json(ex.Message); }
        }
        [HttpPost]
        public ActionResult DeletarPublicacao(int idPublicacao)
        {
            if (!UsuarioLogado())
                throw new Exception("Usuário não encontrado. Faça login para deletar publicação!");
            try
            {
                Dao.Usuarios.RemoverPublicacao(idPublicacao);
                return Json("Sucesso!");
            }
            catch { throw new Exception("Erro ao deletar publicacao"); }
        }
        public bool UsuarioLogado()
        {
            try
            {
                var atual = new Usuario(GetId());
                if (atual == null || atual.Id == 0)
                    return false;
            }
            catch { return false; }
            return true;
        }
    }
}