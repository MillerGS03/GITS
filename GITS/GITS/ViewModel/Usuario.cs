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
        public class Amigo
        {
            public Amigo(Usuario u, bool f)
            {
                Id = u.Id;
                Nome = u.Nome;
                FotoPerfil = u.FotoPerfil;
                XP = u.XP;
                Status = u.Status;
                Insignia = u.Insignia;
                FoiAceito = f;
            }
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
            [Required, StringLength(40)]
            public string Nome { get; set; }
            [Required, StringLength(150)]
            public string FotoPerfil { get; set; }
            public int XP { get; set; }
            [Required, StringLength(30)]
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
        public class Compromisso
        {
            public string Data { get; set; }
            [Required, StringLength(65)]
            public string Titulo { get; set; }
            public string Descricao { get; set; }

            public override bool Equals(object obj)
            {
                var compromisso = obj as Compromisso;
                return compromisso != null &&
                       Data == compromisso.Data &&
                       Titulo == compromisso.Titulo &&
                       Descricao == compromisso.Descricao;
            }

            public override int GetHashCode()
            {
                var hashCode = -1562752441;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Data);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Titulo);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Descricao);
                return hashCode;
            }
        } //Data, Titulo, Descricao
        public class Tarefa : Compromisso
        {
            public Tarefa() { }
            public Tarefa(SqlDataReader s)
            {
                try
                {
                    CodTarefa = Convert.ToInt16(s["CodTarefa"]);
                    Titulo = s["Titulo"].ToString();
                    Descricao = s["Descricao"].ToString();
                    Dificuldade = Convert.ToInt16(s["Dificuldade"]);
                    Urgencia = Convert.ToInt16(s["Urgencia"]);
                    Data = s["Data"].ToString().Substring(0, 10);
                    Meta = null;
                    CodUsuarioCriador = Convert.ToInt16(s["CodUsuarioCriador"]);
                }
                catch { }
            }
            public Tarefa(int codTarefa, string titulo, string descricao, int dificuldade, int urgencia, string data, Meta meta, int codUsuarioCriador)
            {
                CodTarefa = codTarefa;
                Titulo = titulo;
                Descricao = descricao;
                Dificuldade = dificuldade;
                Urgencia = urgencia;
                Data = data;
                Meta = meta;
                CodUsuarioCriador = codUsuarioCriador;
            }
            public Tarefa(string titulo, string descricao, int dificuldade, int urgencia, string data, Meta meta, int codUsuarioCriador)
            {
                Titulo = titulo;
                Descricao = descricao;
                Dificuldade = dificuldade;
                Urgencia = urgencia;
                Data = data;
                Meta = meta;
                CodUsuarioCriador = codUsuarioCriador;
            }
            public Tarefa(string titulo, string descricao, int dificuldade, int urgencia, string data)
            {
                Titulo = titulo;
                Descricao = descricao;
                Dificuldade = dificuldade;
                Urgencia = urgencia;
                Data = data;
            }

            public int CodTarefa { get; set; }
            public int Dificuldade { get; set; }
            public int Urgencia { get; set; }
            public Meta Meta { get; set; }
            public int CodUsuarioCriador { get; set; }

            public override bool Equals(object obj)
            {
                if (!base.Equals(obj))
                    return false;
                var tarefa = obj as Tarefa;
                return tarefa != null &&
                       base.Equals(obj) &&
                       CodTarefa == tarefa.CodTarefa &&
                       Dificuldade == tarefa.Dificuldade &&
                       Urgencia == tarefa.Urgencia &&
                       CodUsuarioCriador == tarefa.CodUsuarioCriador &&
                       EqualityComparer<Meta>.Default.Equals(Meta, tarefa.Meta);
            }

            public override int GetHashCode()
            {
                var hashCode = base.GetHashCode();
                hashCode = hashCode * -1521134295 + base.GetHashCode();
                hashCode = hashCode * -1521134295 + CodTarefa.GetHashCode();
                hashCode = hashCode * -1521134295 + Dificuldade.GetHashCode();
                hashCode = hashCode * -1521134295 + Urgencia.GetHashCode();
                hashCode = hashCode * -1521134295 + CodUsuarioCriador.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<Meta>.Default.GetHashCode(Meta);
                return hashCode;
            }
        }  //Dificuldade, Urgencia, Criador, Meta
        public class Meta : Compromisso
        {
            public Meta(SqlDataReader s)
            {
                try
                {
                    CodMeta = Convert.ToInt16(s["CodMeta"]);
                    Titulo = s["Titulo"].ToString();
                    Descricao = s["Descricao"].ToString();
                    Data = s["Data"].ToString().Substring(0, 10);
                    Progresso = Convert.ToInt16(s["Progresso"]);
                    UltimaInteracao = s["UltimaInteracao"].ToString().Substring(0, 10);
                }
                catch { }
            }
            public Meta()
            {

            }
            public Meta(int codMeta, string titulo, string descricao, string data, int progresso, string ultimaInteracao)
            {
                CodMeta = codMeta;
                Titulo = titulo;
                Descricao = descricao;
                Data = data;
                Progresso = progresso;
                UltimaInteracao = ultimaInteracao;
            }

            public int CodMeta { get; set; }
            public int Progresso { get; set; }
            public string UltimaInteracao { get; set; }

            public override bool Equals(object obj)
            {
                if (!base.Equals(obj))
                    return false;
                var meta = obj as Meta;
                return meta != null &&
                       CodMeta == meta.CodMeta &&
                       Progresso == meta.Progresso &&
                       UltimaInteracao == meta.UltimaInteracao;
            }

            public override int GetHashCode()
            {
                var hashCode = base.GetHashCode();
                hashCode = hashCode * -1521134295 + CodMeta.GetHashCode();
                hashCode = hashCode * -1521134295 + Progresso.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UltimaInteracao);
                return hashCode;
            }
        } //Progresso, UltimaInteracao
        public class Acontecimento : Compromisso
        {
            public Acontecimento()
            {
            }
            public Acontecimento(SqlDataReader s)
            {
                try
                {
                    CodAcontecimento = Convert.ToInt16(s["CodAcontecimento"]);
                    Titulo = s["Titulo"].ToString();
                    Descricao = s["Descricao"].ToString();
                    Data = s["Data"].ToString();
                    Tipo = Convert.ToInt16(s["Tipo"]);
                    CodUsuarioCriador = Convert.ToInt16(s["CodUsuarioCriador"]);
                }
                catch { }
            }
            public Acontecimento(int codAcontecimento, string titulo, string descricao, string data, int tipo, int codUsuarioCriador)
            {
                CodAcontecimento = codAcontecimento;
                Titulo = titulo;
                Descricao = descricao;
                Data = data;
                Tipo = tipo;
                CodUsuarioCriador = codUsuarioCriador;
            }

            public int CodAcontecimento { get; set; }
            public int Tipo { get; set; }
            public int CodUsuarioCriador { get; set; }

            public override bool Equals(object obj)
            {
                if (!base.Equals(obj))
                    return false;
                var acontecimento = obj as Acontecimento;
                return acontecimento != null &&
                       base.Equals(obj) &&
                       CodAcontecimento == acontecimento.CodAcontecimento &&
                       Tipo == acontecimento.Tipo &&
                       CodUsuarioCriador == acontecimento.CodUsuarioCriador;
            }

            public override int GetHashCode()
            {
                var hashCode = base.GetHashCode();
                hashCode = hashCode * -1521134295 + base.GetHashCode();
                hashCode = hashCode * -1521134295 + CodAcontecimento.GetHashCode();
                hashCode = hashCode * -1521134295 + Tipo.GetHashCode();
                hashCode = hashCode * -1521134295 + CodUsuarioCriador.GetHashCode();
                return hashCode;
            }
        } //Tipo, Criador
          // 0 - Tarefa
          // 1 - Solicitação de amizade
          // 2 - Marcado
        public class Notificacao
        {
            public Notificacao(SqlDataReader s)
            {
                Id = Convert.ToInt32(s["Id"]);
                IdUsuarioReceptor = Convert.ToInt32(s["IdUsuarioReceptor"]);
                IdUsuarioTransmissor = Convert.ToInt32(s["IdUsuarioTransmissor"]);
                Tipo = Convert.ToInt32(s["Tipo"]);
                IdCoisa = Convert.ToInt32(s["IdCoisa"]);
                JaViu = Convert.ToInt32(s["JaViu"]) == 1;
            }
            public Notificacao()
            {
            }

            public Notificacao(int id, int idUsuarioReceptor, int idUsuarioTransmissor, int tipo, int idCoisa, bool jaViu)
            {
                Id = id;
                IdUsuarioReceptor = idUsuarioReceptor;
                IdUsuarioTransmissor = idUsuarioTransmissor;
                Tipo = tipo;
                IdCoisa = idCoisa;
                JaViu = jaViu;
            }
            public Notificacao(int idUsuarioReceptor, int idUsuarioTransmissor, int tipo, int idCoisa, bool jaViu)
            {
                IdUsuarioReceptor = idUsuarioReceptor;
                IdUsuarioTransmissor = idUsuarioTransmissor;
                Tipo = tipo;
                IdCoisa = idCoisa;
                JaViu = jaViu;
            }
            public Notificacao(Amigo a, int id)
            {
                IdUsuarioReceptor = id;
                IdUsuarioTransmissor = a.Id;
                Tipo = 1;
                IdCoisa = a.Id;
                JaViu = false;
            }
            public Notificacao(Tarefa t, int idReceptor, int idTransmissor)
            {
                IdUsuarioReceptor = idReceptor;
                IdUsuarioTransmissor = idTransmissor;
                Tipo = 0;
                IdCoisa = t.CodTarefa;
                JaViu = false;
            }

            public int Id { get; set; }
            public int IdUsuarioReceptor { get; set; }
            public int IdUsuarioTransmissor { get; set; }
            public int Tipo { get; set; }
            public int IdCoisa { get; set; }
            public bool JaViu { get; set; }
            public override string ToString()
            {
                string ret = $"{Dao.Usuarios.GetUsuario(IdUsuarioTransmissor).Nome}";
                switch (Tipo)
                {
                    case 0:
                        ret += $" te convidou para participar da tarefa \"{Dao.Eventos.Tarefa(IdCoisa).Titulo}\"";
                        break;
                    case 1:
                        ret += $" te enviou uma solicitação de amizade";
                        break;
                    case 2:
                        ret += $" te marcou em uma publicação";
                        break;
                }
                return ret;
            }
            public string Link
            {
                get
                {
                    string l = "/";
                    switch(Tipo)
                    {
                        case 0:
                            l += $"tarefas/{IdCoisa}";
                            break;
                        case 1:
                            l += $"perfil/{IdUsuarioTransmissor}";
                            break;
                        case 2:
                            l += $"publicacoes/{IdCoisa}";
                            break;
                    }
                    return l;
                }
            }
            public string ToHtml
            {
                get
                {
                    string html = "";
                    html += $"<li><a href=\"{Link}\">{ToString()}</a></li>";
                    return html;
                }
            }

            public override bool Equals(object obj)
            {
                var notificacao = obj as Notificacao;
                return notificacao != null &&
                       Id == notificacao.Id &&
                       IdUsuarioReceptor == notificacao.IdUsuarioReceptor &&
                       IdUsuarioTransmissor == notificacao.IdUsuarioTransmissor &&
                       Tipo == notificacao.Tipo &&
                       IdCoisa == notificacao.IdCoisa &&
                       JaViu == notificacao.JaViu;
            }

            public override int GetHashCode()
            {
                var hashCode = 724349007;
                hashCode = hashCode * -1521134295 + Id.GetHashCode();
                hashCode = hashCode * -1521134295 + IdUsuarioReceptor.GetHashCode();
                hashCode = hashCode * -1521134295 + IdUsuarioTransmissor.GetHashCode();
                hashCode = hashCode * -1521134295 + Tipo.GetHashCode();
                hashCode = hashCode * -1521134295 + IdCoisa.GetHashCode();
                hashCode = hashCode * -1521134295 + JaViu.GetHashCode();
                return hashCode;
            }
        }
        public class Publicacao
        {
            public Publicacao(int idUsuario, string titulo, string descricao, DateTime data)
            {
                IdUsuario = idUsuario;
                Titulo = titulo;
                Descricao = descricao;
                Data = data;
            }

            public Publicacao(int idPublicacao, int idUsuario, string titulo, string descricao, DateTime data)
            {
                IdPublicacao = idPublicacao;
                IdUsuario = idUsuario;
                Titulo = titulo;
                Descricao = descricao;
                Data = data;
            }

            public int IdPublicacao { get; set; }
            public int IdUsuario { get; set; }
            public string Titulo { get; set; }
            public string Descricao { get; set; }
            public DateTime Data { get; set; }
            public string DataFormatada { get => "06/05/2019"; } // Arrumar isso aqui

            public override bool Equals(object obj)
            {
                return obj is Publicacao publicacao &&
                       IdUsuario == publicacao.IdUsuario &&
                       Titulo == publicacao.Titulo &&
                       Descricao == publicacao.Descricao &&
                       Data == publicacao.Data;
            }

            public override int GetHashCode()
            {
                var hashCode = 597487799;
                hashCode = hashCode * -1521134295 + IdUsuario.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Titulo);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Descricao);
                hashCode = hashCode * -1521134295 + Data.GetHashCode();
                return hashCode;
            }
        }

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
            Amigos = new List<Amigo>();
            Acontecimentos = new List<Acontecimento>();
            Tarefas = new List<Tarefa>();
            Metas = new List<Meta>();
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
                Tarefas = u.Tarefas;
                Acontecimentos = u.Acontecimentos;
                Metas = u.Metas;
                Notificacoes = u.Notificacoes;
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
                usuarioAtual.Tarefas = new List<Tarefa>();
                usuarioAtual.Metas = new List<Meta>();
                usuarioAtual.Notificacoes = new List<Notificacao>();
                usuarioAtual.Acontecimentos = new List<Acontecimento>();
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
                usuarioAtual.Tarefas = Dao.Eventos.Tarefas(usuarioAtual.Id, true);
                usuarioAtual.Metas = Dao.Eventos.Metas(usuarioAtual.Id);
                usuarioAtual.Acontecimentos = Dao.Eventos.Acontecimentos(usuarioAtual.Id);
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
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Tarefa>>.Default.GetHashCode(Tarefas);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Meta>>.Default.GetHashCode(Metas);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Notificacao>>.Default.GetHashCode(Notificacoes);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Acontecimento>>.Default.GetHashCode(Acontecimentos);
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