using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace GITS.ViewModel
{
    public class Usuario
    {
        public class Amigo
        {
            public Amigo()
            {
            }

            public Amigo(int id, string nome, string fotoPerfil, int xP, string status, int insignia, bool aceito)
            {
                Id = id;
                Nome = nome;
                FotoPerfil = fotoPerfil;
                XP = xP;
                Status = status;
                Insignia = insignia;
                FoiAceito = aceito;
            }

            public int Id { get; set; }
            public string Nome { get; set; }
            public string FotoPerfil { get; set; }
            public int XP { get; set; }
            public string Status { get; set; }
            public int Insignia { get; set; }
            public bool FoiAceito { get; set; }

            public override bool Equals(object obj)
            {
                var amigo = obj as Amigo;
                return amigo != null &&
                       Id == amigo.Id &&
                       Nome == amigo.Nome &&
                       FotoPerfil == amigo.FotoPerfil &&
                       XP == amigo.XP &&
                       Status == amigo.Status &&
                       Insignia == amigo.Insignia &&
                       FoiAceito == amigo.FoiAceito;
            }

            public override int GetHashCode()
            {
                var hashCode = 321779470;
                hashCode = hashCode * -1521134295 + Id.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nome);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FotoPerfil);
                hashCode = hashCode * -1521134295 + XP.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Status);
                hashCode = hashCode * -1521134295 + Insignia.GetHashCode();
                hashCode = hashCode * -1521134295 + FoiAceito.GetHashCode();
                return hashCode;
            }
        }
        public class Tarefa
        {
            public Tarefa() {}
            public Tarefa(string titulo, string descricao, int dificuldade, int urgencia, DateTime data)
            {
                Titulo = titulo;
                Descricao = descricao;
                Dificuldade = dificuldade;
                Urgencia = urgencia;
                Data = data;
            }

            public int CodTarefa { get; set; }
            public string Titulo { get; set; }
            public string Descricao { get; set; }
            public int Dificuldade { get; set; }
            public int Urgencia { get; set; }
            public DateTime Data { get; set; }

            public override bool Equals(object obj)
            {
                var tarefa = obj as Tarefa;
                return tarefa != null &&
                       Titulo == tarefa.Titulo &&
                       Descricao == tarefa.Descricao &&
                       Dificuldade == tarefa.Dificuldade &&
                       Urgencia == tarefa.Urgencia &&
                       Data == tarefa.Data;
            }

            public override int GetHashCode()
            {
                var hashCode = -265687277;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Titulo);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Descricao);
                hashCode = hashCode * -1521134295 + Dificuldade.GetHashCode();
                hashCode = hashCode * -1521134295 + Urgencia.GetHashCode();
                hashCode = hashCode * -1521134295 + Data.GetHashCode();
                return hashCode;
            }
        }
        public Usuario(int id, string codUsuario, string email, string nome, string fotoPerfil, int xP, string status, int insignia, double dinheiro, string titulo, int temaSite, int decoracao)
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

        }

        public int Id { get; set; }
        public string CodUsuario { get; set; }
        public string Email { get; set; }
        public string Nome { get; set; }
        public string FotoPerfil { get; set; }
        public List<Amigo> Amigos { get; set; }
        public int XP { get; set; }
        public int Level { get; set; }
        public string Status { get; set; }
        public int Insignia { get; set; }
        public double Dinheiro { get; set; }
        public string Titulo { get; set; }
        public int TemaSite { get; set; }
        public int Decoracao { get; set; }
        public List<Tarefa> Tarefas { get; set; }

        internal static Usuario GetLoginInfo(ClaimsIdentity identity)
        {
            if (identity.Claims.Count() == 0 || identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email) == null)
            {
                return null;
            }
            var usuarios = new Dao().Usuarios;
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
                usuarioAtual.Tarefas = new List<Tarefa>();
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
                usuarioAtual.Amigos = usuarios.Amigos(usuarioAtual.Id);
                usuarioAtual.Tarefas = usuarios.Tarefas(usuarioAtual.Id);
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
            for (int i = 0; i < Tarefas.Count; i++)
                if (!Tarefas[i].Equals(u.Tarefas[i]))
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