using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace GITS.ViewModel
{
    // 0 - Tarefa
    // 1 - Solicitação de amizade
    // 2 - Marcado
    // 3 - aceitou notificação
    // 4 - requisicao admin
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
        public Notificacao(Publicacao p, int id)
        {
            IdUsuarioReceptor = id;
            IdUsuarioTransmissor = p.IdUsuario;
            Tipo = 2;
            IdCoisa = p.IdPublicacao;
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
            string ret = "";
            var uT = Dao.Usuarios.GetUsuario(IdUsuarioTransmissor);
            if (uT != null)
                ret = $"{uT.Nome}";
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
                case 3:
                    ret += $" aceitou sua solicitação de amizade";
                    break;
                case 4:
                    ret += $" quer se tornar admin da tarefa \"{Dao.Eventos.Tarefa(IdCoisa).Titulo}\"";
                    break;
            }
            return ret;
        }
        public string Link
        {
            get
            {
                string l = "/";
                switch (Tipo)
                {
                    case 0:
                        l += $"tarefas/{IdCoisa}";
                        break;
                    case 1:
                        l += $"perfil/{IdUsuarioTransmissor}";
                        break;
                    case 2:
                        l += $"publicacao/{IdCoisa}";
                        break;
                    case 3:
                        l += $"perfil/{IdUsuarioTransmissor}";
                        break;
                    case 4:
                        l += $"tarefas/{IdCoisa}";
                        break;
                }
                return l;
            }
        }
        public string ToHtml
        {
            get
            {
                string html = "", btns = "";
                switch (Tipo)
                {
                    case 1:
                        btns = $"<button onclick=\"$.post({{url: \'/AceitarSolicitacaoDeAmizade\', data: {{idNotificacao: {Id}, codAmizade: {IdCoisa}, n: {{IdUsuarioReceptor: {IdUsuarioTransmissor}, IdUsuarioTransmissor: {IdUsuarioReceptor}, Tipo: 3, IdCoisa: {IdCoisa}, JaViu: false}} }} }}, function(ret) {{tratar(JSON.parte(ret))}});\">Aceitar</button>";
                        btns += $"<button onclick=\"$.post({{url: \'RecusarSolicitacaoDeAmizade\', data: {{idNotificacao: {Id}, codAmizade: {IdCoisa}}} }}, function(ret) {{tratar(JSON.parse(ret))}});\">Recusar</button>";
                        html += $"<li><a href=\"{Link}\">{ToString()}</a>{(JaViu ? "" : btns)}</li>";
                        break;
                    case 4:
                        btns = $"<button onclick=\"$.post({{url: \'/AceitarAdmTarefa\', data: {{codTarefa: {IdCoisa}, idUsuario: {IdUsuarioTransmissor}, codNotif: {Id} }}, async: false }});\">Aceitar</button>";
                        btns += $"<button onclick=\"$.post({{url: \'RecusarAdmTarefa\', data: {{codNotif: {Id} }}, async: false }});\">Recusar</button>";
                        html += $"<li><a href=\"{Link}\">{ToString()}</a>{(JaViu ? "" : btns)}</li>";
                        break;
                    default:
                        html = $"<li><a href=\"{Link}\">{ToString()}</a></li>";
                        break;
                }
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
}