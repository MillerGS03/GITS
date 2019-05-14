using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace GITS.ViewModel
{
    public class Usuario
    {

        public Usuario(SqlDataReader dr)
        {
            try
            {
                Id = Convert.ToInt16(dr["Id"]);
                CodUsuario = dr["CodUsuario"].ToString();
                Email = dr["Email"].ToString();
                Nome = dr["Nome"].ToString();
                FotoPerfil = dr["FotoPerfil"].ToString();
                XP = Convert.ToInt16(dr["XP"]);
                Status = dr["_Status"].ToString();
                Insignia = Convert.ToInt16(dr["Insignia"]);
                Dinheiro = Convert.ToDouble(dr["Dinheiro"]);
                Titulo = dr["Titulo"].ToString();
                TemaSite = Convert.ToInt16(dr["TemaSite"]);
                Decoracao = Convert.ToInt16(dr["Decoracao"]);
            }
            catch { }
        }
        public Usuario(int id, string codUsuario, string email, string nome, string fotoPerfil, int xP, string status, int insignia, double dinheiro, string titulo, int temaSite, int decoracao) : this()
        {
            Id = id;
            CodUsuario = codUsuario;
            Email = email;
            Nome = nome;
            FotoPerfil = fotoPerfil;
            XP = xP;
            Status = status;
            Insignia = insignia;
            Dinheiro = dinheiro;
            Titulo = titulo;
            TemaSite = temaSite;
            Decoracao = decoracao;
        }
        public Usuario()
        {
            Amigos = new List<Amigo>();
            Solicitacoes = new List<Amigo>();
            Acontecimentos = new List<Acontecimento>();
            Tarefas = new List<Tarefa>();
            Metas = new List<Meta>();
            Itens = new List<Item>();
        }
        public Usuario(int id) : this(Dao.Usuarios.GetUsuario(id))
        {
        }
        public Usuario(Usuario u)
        {
            if (u != null)
            {
                Id = u.Id;
                CodUsuario = u.CodUsuario;
                Email = u.Email;
                Nome = u.Nome;
                FotoPerfil = u.FotoPerfil;
                XP = u.XP;
                Status = u.Status;
                Insignia = u.Insignia;
                Dinheiro = u.Dinheiro;
                Titulo = u.Titulo;
                TemaSite = u.TemaSite;
                Decoracao = u.Decoracao;
                Amigos = u.Amigos;
                Solicitacoes = u.Solicitacoes;
                Tarefas = u.Tarefas;
                Acontecimentos = u.Acontecimentos;
                Metas = u.Metas;
                Notificacoes = u.Notificacoes;
                Itens = u.Itens;
            }
        }

        public int Id { get; set; }
        [Required, StringLength(30)]
        public string CodUsuario { get; set; }
        [Required, StringLength(35)]
        public string Email { get; set; }
        [Required, StringLength(40)]
        public string Nome { get; set; }
        [Required, StringLength(150)]
        public string FotoPerfil { get; set; }
        public List<Amigo> Amigos { get; set; }
        public List<Amigo> Solicitacoes { get; set; }
        public int XP { get; set; }
        public int Level { get; set; }
        [Required, StringLength(50)]
        public string Status { get; set; }
        public int Insignia { get; set; }
        public double Dinheiro { get; set; }
        [Required, StringLength(15)]
        public string Titulo { get; set; }
        public int TemaSite { get; set; }
        public int Decoracao { get; set; }
        public List<Tarefa> Tarefas { get; set; }
        public List<Meta> Metas { get; set; }
        public List<Acontecimento> Acontecimentos { get; set; }
        public List<Notificacao> Notificacoes { get; set; }
        public List<Item> Itens { get; set; }

        internal static Usuario GetLoginInfo(ClaimsIdentity identity)
        {
            if (identity.Claims.Count() == 0 || identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email) == null)
            {
                return null;
            }
            var usuarios = Dao.Usuarios;
            string codAtual = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var usuarioAtual = usuarios.ToList().Find(u => u.CodUsuario == codAtual);
            string foto = "";
            var accessToken = identity.Claims.Where(c => c.Type.Equals("urn:google:accesstoken")).Select(c => c.Value).FirstOrDefault();
            Uri apiRequestUri = new Uri("https://www.googleapis.com/oauth2/v2/userinfo?access_token=" + accessToken);
            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString(apiRequestUri);
                dynamic result = JsonConvert.DeserializeObject(json);
                foto = result.picture;
            }
            if (usuarioAtual == null) //adicionar usuario que nao existe ainda
            {
                usuarioAtual = new Usuario();
                usuarioAtual.CodUsuario = codAtual;
                usuarioAtual.Nome = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
                usuarioAtual.Email = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
                usuarioAtual.Amigos = new List<Amigo>();
                usuarioAtual.Solicitacoes = new List<Amigo>();
                usuarioAtual.Tarefas = new List<Tarefa>();
                usuarioAtual.Metas = new List<Meta>();
                usuarioAtual.Notificacoes = new List<Notificacao>();
                usuarioAtual.Acontecimentos = new List<Acontecimento>();
                usuarioAtual.Itens = new List<Item>();
                usuarioAtual.XP = 0;
                usuarioAtual.Status = "Bom dia!";
                usuarioAtual.Insignia = 0;
                usuarioAtual.Dinheiro = 0;
                usuarioAtual.Titulo = "Novato";
                usuarioAtual.TemaSite = 1;
                usuarioAtual.FotoPerfil = foto;
                usuarioAtual.Id = usuarios.Add(usuarioAtual);
            }
            else
            {
                string email = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
                string nome = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
                if (usuarioAtual.Email != email || usuarioAtual.Nome != nome || usuarioAtual.FotoPerfil != foto)
                {
                    usuarioAtual.Email = email;
                    usuarioAtual.Nome = nome;
                    usuarioAtual.FotoPerfil = foto;
                    usuarios.Update(usuarioAtual);
                }
                usuarioAtual.Amigos = usuarios.Amigos(usuarioAtual.Id, true);
                usuarioAtual.Solicitacoes = usuarios.Amigos(usuarioAtual.Id, false);
                usuarioAtual.Tarefas = Dao.Eventos.Tarefas(usuarioAtual.Id, true);
                usuarioAtual.Metas = Dao.Eventos.Metas(usuarioAtual.Id);
                usuarioAtual.Acontecimentos = Dao.Eventos.Acontecimentos(usuarioAtual.Id);
                usuarioAtual.Itens = Dao.Itens.GetItensDeUsuario(usuarioAtual.Id);
                usuarioAtual.Notificacoes = usuarios.Notificacoes(usuarioAtual.Id);
            }
            return usuarioAtual;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj == this)
                return true;
            if (obj.GetType() != typeof(Usuario))
                return false;
            Usuario u = obj as Usuario;
            if (Nome != u.Nome)
                return false;
            if (Email != u.Email)
                return false;
            if (FotoPerfil != u.FotoPerfil)
                return false;
            if (Status != u.Status)
                return false;
            if (Insignia != u.Insignia)
                return false;
            if (TemaSite != u.TemaSite)
                return false;
            if (Decoracao != u.Decoracao)
                return false;
            if (XP != u.XP)
                return false;
            if (CodUsuario != u.CodUsuario)
                return false;
            if (Amigos.Count != u.Amigos.Count)
                return false;
            for (int i = 0; i < Amigos.Count; i++)
                if (Amigos[i].Id != u.Amigos[i].Id || Amigos[i].FoiAceito != u.Amigos[i].FoiAceito || Amigos[i].FotoPerfil != u.Amigos[i].FotoPerfil)
                    return false;
            for (int i = 0; i < Solicitacoes.Count; i++)
                if (Solicitacoes[i].Id != u.Solicitacoes[i].Id || Solicitacoes[i].FoiAceito != u.Solicitacoes[i].FoiAceito || Solicitacoes[i].FotoPerfil != u.Solicitacoes[i].FotoPerfil)
                    return false;
            if (Tarefas.Count != u.Tarefas.Count)
                return false;
            for (int i = 0; i < Tarefas.Count; i++)
                if (!Tarefas[i].Equals(u.Tarefas[i]))
                    return false;
            if (Metas.Count != u.Metas.Count)
                return false;
            for (int i = 0; i < Metas.Count; i++)
                if (!Metas[i].Equals(u.Metas[i]))
                    return false;
            if (Acontecimentos.Count != u.Acontecimentos.Count)
                return false;
            for (int i = 0; i < Acontecimentos.Count; i++)
                if (!Acontecimentos[i].Equals(u.Acontecimentos[i]))
                    return false;
            if (Notificacoes.Count != u.Notificacoes.Count)
                return false;
            for (int i = 0; i < Notificacoes.Count; i++)
                if (!Notificacoes[i].Equals(u.Notificacoes[i]))
                    return false;
            if (Itens.Count != u.Itens.Count)
                return false;
            for (int i = 0; i < Itens.Count; i++)
                if (!Itens[i].Equals(u.Itens[i]))
                    return false;
            return true;
        }
        public override int GetHashCode()
        {
            var hashCode = -1080711052;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CodUsuario);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Email);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nome);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FotoPerfil);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Amigo>>.Default.GetHashCode(Amigos);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Amigo>>.Default.GetHashCode(Solicitacoes);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Tarefa>>.Default.GetHashCode(Tarefas);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Meta>>.Default.GetHashCode(Metas);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Notificacao>>.Default.GetHashCode(Notificacoes);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Acontecimento>>.Default.GetHashCode(Acontecimentos);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Item>>.Default.GetHashCode(Itens);
            hashCode = hashCode * -1521134295 + XP.GetHashCode();
            hashCode = hashCode * -1521134295 + Level.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Status);
            hashCode = hashCode * -1521134295 + Insignia.GetHashCode();
            hashCode = hashCode * -1521134295 + Dinheiro.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Titulo);
            hashCode = hashCode * -1521134295 + TemaSite.GetHashCode();
            hashCode = hashCode * -1521134295 + Decoracao.GetHashCode();
            return hashCode;
        }
    }
}