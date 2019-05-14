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
                    int id = GetId();
                    if (ViewBag.Usuario == null || ViewBag.Usuario.Id != id)
                        ViewBag.Usuario = new Usuario(id);
                    if (ViewBag.Usuario.Tarefas != null)
                    {
                        if (ViewBag.Feed == null || ViewBag.Usuario.Id != id)
                        {
                            List<Publicacao> pubs = Dao.Usuarios.PublicacoesRelacionadasA(ViewBag.Usuario.Id);
                            if (pubs.Find(p => p.IdPublicacao == 0 && p.UsuariosMarcados == null) == null)
                            {
                                ViewBag.Feed = pubs;
                                return View("Index");
                            }
                            else
                                throw new Exception();
                        }
                        else
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
                ViewBag.SolicitacaoAtiva = false;
                ViewBag.ConvidouVoce = false;
                ViewBag.IsLoggedIn = false;

                Usuario usuarioLogado = null;

                if (Request.Cookies["user"] != null)
                {
                    usuarioLogado = new Usuario(GetId());
                    ViewBag.IsLoggedIn = true;
                    ViewBag.IdUsuarioLogado = usuarioLogado.Id;
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
                        else
                        {
                            foreach (Amigo amigo in usuario.Solicitacoes)
                                if (amigo.Id == usuarioLogado.Id)
                                {
                                    ViewBag.SolicitacaoAtiva = true;
                                    ViewBag.ConvidouVoce = amigo.ConvidouVoce;
                                    break;
                                }
                        }
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
                if (ViewBag.Publicacao != null && ViewBag.Publicacao.IdPublicacao != 0)
                {
                    ViewBag.UsuarioCriador = Dao.Usuarios.GetUsuario(ViewBag.Publicacao.IdUsuario);
                    try
                    {
                        ViewBag.IdUsuarioLogado = GetId();
                        ViewBag.IsYourself = ViewBag.Publicacao.IdUsuario == ViewBag.IdUsuarioLogado;
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
        public string GetTema()
        {
            try
            {
                return new JavaScriptSerializer().Serialize(Dao.Usuarios.Tema(GetId()));
            }
            catch { return ""; }
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
                if (convites != null)
                    Array.Sort(convites, StringComparer.InvariantCulture);
                else
                    convites = new string[1];
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
            catch { throw new Exception("Usuário não encontrado. Faça login para removar a amizade!"); }
            try
            {
                Dao.Usuarios.RemoverAmizade(atual.Id, idUsuario);
                return Json("Sucesso");
            }
            catch (Exception ex) { return Json(ex.Message); }
        }
        [HttpPost]
        public ActionResult AceitarSolicitacaoDeAmizadeId(int idUsuario)
        {
            Usuario atual;
            try
            {
                atual = new Usuario(GetId());
                if (atual == null)
                    throw new Exception();
            }
            catch { throw new Exception("Usuário não encontrado. Faça login para aceitar a solicitação!"); }

            int idAmizade = Dao.Usuarios.AceitarAmizade(atual.Id, idUsuario);
            Dao.Usuarios.VisualizarNotificacao(Dao.Exec($"select Id from Notificacao where Tipo=1 and IdCoisa={idAmizade}", typeof(int)));
            return Json("Sucesso");
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
        public ActionResult RecusarSolicitacaoDeAmizadeId(int idUsuario)
        {
            Usuario atual;
            try
            {
                atual = new Usuario(GetId());
                if (atual == null)
                    throw new Exception();
            }
            catch { throw new Exception("Usuário não encontrado. Faça login para aceitar a solicitação!"); }

            int idAmizade = Dao.Usuarios.RecusarAmizade(atual.Id, idUsuario);
            Dao.Usuarios.VisualizarNotificacao(Dao.Exec($"select Id from Notificacao where Tipo=1 and IdCoisa={idAmizade}", typeof(int)));
            return Json("Sucesso");
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
                Dao.Usuarios.Publicar(new Publicacao(atual.Id, titulo, descricao, DateTime.Now, 0, null), idsUsuariosMarcados);
                return Json("Sucesso");
            }
            catch (Exception ex) { return Json(ex.Message); }
        }
        [HttpPost]
        public ActionResult DeletarPublicacao(int idPublicacao)
        {
            Usuario atual;
            try
            {
                atual = new Usuario(GetId());
                if (atual == null || atual.Id == 0)
                    throw new Exception();
            }
            catch { throw new Exception("Usuário não encontrado. Faça login para deletar publicação!"); }
            try
            {
                Dao.Usuarios.RemoverPublicacao(idPublicacao, atual.Id);
                return Json("Sucesso!");
            }
            catch { throw new Exception("Erro ao deletar publicacao"); }
        }
        [HttpPost]
        public ActionResult EditarPublicacao(int idPublicacao, string novoTitulo, string novoConteudo)
        {
            Usuario atual;
            try
            {
                atual = new Usuario(GetId());
                if (atual == null || atual.Id == 0)
                    throw new Exception();
            }
            catch { throw new Exception("Usuário não encontrado. Faça login para atualizar publicação!"); }
            try
            {
                Dao.Usuarios.AtualizarPublicacao(idPublicacao, atual.Id, novoTitulo, novoConteudo);
                return Json("Sucesso!");
            }
            catch { throw new Exception("Erro ao atualizar publicacao"); }
        }
        public ActionResult Responder(int idPublicacao, string descricao)
        {
            Usuario atual;
            try
            {
                atual = new Usuario(GetId());
                if (atual == null || atual.Id == 0)
                    throw new Exception();
            }
            catch { throw new Exception("Usuário não encontrado. Faça login para responder publicação!"); }
            try
            {
                Dao.Usuarios.Publicar(new Publicacao(atual.Id, "Resposta a " + Dao.Usuarios.Publicacao(idPublicacao).Usuario.Nome, descricao, DateTime.Now, 0, idPublicacao), new int[0]);
                return Json("Sucesso!");
            }
            catch { throw new Exception("Erro ao responder publicacao"); }
        }
        public ActionResult GostarDe(int idPublicacao)
        {
            Usuario atual;
            try
            {
                atual = new Usuario(GetId());
                if (atual == null || atual.Id == 0)
                    throw new Exception();
            }
            catch { throw new Exception("Usuário não encontrado. Faça login para gostar da publicação!"); }
            try
            {
                Dao.Usuarios.GostarDe(idPublicacao, atual.Id);
                return Json("Sucesso!");
            }
            catch { throw new Exception("Erro ao gostar da publicacao"); }
        }
        public ActionResult DesgostarDe(int idPublicacao)
        {
            Usuario atual;
            try
            {
                atual = new Usuario(GetId());
                if (atual == null || atual.Id == 0)
                    throw new Exception();
            }
            catch { throw new Exception("Usuário não encontrado. Faça login para desgostar da publicação!"); }
            try
            {
                Dao.Usuarios.DesgostarDe(idPublicacao, atual.Id);
                return Json("Sucesso!");
            }
            catch { throw new Exception("Erro ao desgostar da publicacao"); }
        }
        public string GetItensDeTipoEUsuario(byte tipo, string criptId)
        {
            try
            {
                return new JavaScriptSerializer().Serialize(Dao.Itens.GetItensDeTipoEUsuario(tipo, int.Parse(StringCipher.Decrypt(criptId, senhaCriptografia))));
            }
            catch { return ""; }
        }
        public string GetItensDeTipo(byte tipo)
        {
            try
            {
                return new JavaScriptSerializer().Serialize(Dao.Itens.GetItensDeTipo(tipo));
            }
            catch { return ""; }
        }
        public string GetItem(int id)
        {
            try
            {
                if (id != -1)
                    return new JavaScriptSerializer().Serialize(Dao.Itens.GetItem(id));
                else
                    return "";
            }
            catch
            {
                return "";
            }
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
        [HttpPost]
        public void ComprarItem(int idItem, int tipo)
        {
            try
            {
                Dao.Itens.Comprar(GetId(), idItem);
            }
            catch { }
        }
        [HttpPost]
        public void EquiparItem(int idItem, int tipo)
        {
            try
            {
                if (tipo > 0)
                {
                    string comando = "";
                    switch (tipo)
                    {
                        case 1:
                            comando = $"Decoracao = {idItem}";
                            break;
                        case 2:
                            comando = $"Insignia = {idItem}";
                            break;
                        case 3:
                            comando = $"TemaSite = {idItem}";
                            break;
                    }
                    Dao.Exec($"update Usuario set {comando} where Id = {GetId()}");
                }
                else
                {
                    Item i = Dao.Itens.GetItem(idItem);
                    if (i.Conteudo.Length > 1)
                        Dao.Itens.TrocarTitulo(idItem, GetId());
                    else
                        Dao.Itens.TrocarTitulo(i.Conteudo, GetId());
                }
            }
            catch { }
        }
        [HttpPost]
        public void DesquiparItem(int idItem, int tipo)
        {
            try
            {
                if (tipo != 3)
                {
                    switch (tipo)
                    {
                        case 0:
                            Item i = Dao.Itens.GetItem(idItem);
                            if (i.Conteudo.Length > 1)
                                Dao.Itens.TrocarTitulo(-1, GetId());
                            else
                                Dao.Itens.TirarEfeitoDeTitulo(i.Conteudo, GetId());
                            break;
                        case 1:
                            Dao.Exec($"update Usuario set Decoracao = -1 where Id = {GetId()}");
                            break;
                        case 2:
                            Dao.Exec($"update Usuario set Insignia = -1 where Id = {GetId()}");
                            break;
                    }
                }
                else
                    EquiparItem(5, 3);
            }
            catch { }
        }
    }
}