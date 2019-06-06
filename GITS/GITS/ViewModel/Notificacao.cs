using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace GITS.ViewModel
{
    // 0 - convite Tarefa                   OK
    // 1 - convite acontecimento            1/2 OK
    // 2 - Solicitação de amizade           OK
    // 3 - Marcado                          OK
    // 4 - aceitou notificação              OK
    // 5 - requisicao admin Tarefa          OK
    // 6 - requisicao admin Acontecimento   OK
    // 7 - requisicao entrar Tarefa         1/2 OK
    // 8 - requisicao entrar Acontecimento  1/2 OK
    // 9 - saiu de Tarefa                   1/2 OK
    // 10 - saiu de Acontecimento           1/2 OK
    // 11 - se tornou adm de tarefa         1/2 OK
    // 12 - se tornou adm de acontecimento  1/2 OK
    public class Notificacao
    {
        #region Construtores
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
            Tipo = 3;
            IdCoisa = p.IdPublicacao;
            JaViu = false;
        }
        #endregion

        #region Propriedades
        public int Id { get; set; }
        public int IdUsuarioReceptor { get; set; }
        public int IdUsuarioTransmissor { get; set; }
        public int Tipo { get; set; }
        public int IdCoisa { get; set; }
        public bool JaViu { get; set; }
        #endregion
        public override string ToString()
        {
            string ret = "";
            if (IdUsuarioTransmissor != 0 && Tipo < 11)
            {
                var uT = Dao.Usuarios.GetUsuario(IdUsuarioTransmissor);
                if (uT != null)
                    ret = $"{uT.Nome}";
            }
            switch (Tipo)
            {
                case 0:
                    ret += $" te convidou para participar da tarefa \"{Dao.Eventos.Tarefa(IdCoisa, IdUsuarioReceptor).Titulo}\"";
                    break;
                case 1:
                    ret += $" te convidou para participar do acontecimento \"{Dao.Eventos.Acontecimento(IdCoisa).Titulo}\"";
                    break;
                case 2:
                    ret += $" te enviou uma solicitação de amizade";
                    break;
                case 3:
                    ret += $" te marcou em uma publicação";
                    break;
                case 4:
                    ret += $" aceitou sua solicitação de amizade";
                    break;
                case 5:
                    ret += $" quer se tornar admin da tarefa \"{Dao.Eventos.Tarefa(IdCoisa, IdUsuarioReceptor).Titulo}\"";
                    break;
                case 6:
                    ret += $" quer se tornar admin do acontecimento \"{Dao.Eventos.Acontecimento(IdCoisa).Titulo}\"";
                    break;
                case 7:
                    ret += $" quer se tornar fazer parte da tarefa \"{Dao.Eventos.Tarefa(IdCoisa, IdUsuarioReceptor).Titulo}\"";
                    break;
                case 8:
                    ret += $" quer se tornar fazer parte do acontecimento \"{Dao.Eventos.Acontecimento(IdCoisa).Titulo}\"";
                    break;
                case 9:
                    ret += $" saiu da tarefa \"{Dao.Eventos.Tarefa(IdCoisa, IdUsuarioReceptor).Titulo}\"";
                    break;
                case 10:
                    ret += $" saiu do acontecimento \"{Dao.Eventos.Acontecimento(IdCoisa).Titulo}\"";
                    break;
                case 11:
                    ret += $"Você agora é administrador da tarefa \"{Dao.Eventos.Tarefa(IdCoisa, IdUsuarioReceptor).Titulo}\"";
                    break;
                case 12:
                    ret += $"Você agora é administrador do acontecimento \"{Dao.Eventos.Acontecimento(IdCoisa).Titulo}\"";
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
                    case 5:
                    case 7:
                    case 9:
                    case 11:
                        l += $"tarefas/{IdCoisa}";
                        break;
                    case 1:
                    case 6:
                    case 8:
                    case 10:
                    case 12:
                        l += $"acontecimentos/{IdCoisa}";
                        break;
                    case 2:
                    case 4:
                        l += $"perfil/{IdUsuarioTransmissor}";
                        break;
                    case 3:
                        l += $"publicacao/{IdCoisa}";
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
                    case 2:
                        btns = $"<button onclick=\"$.post({{url: \'/AceitarSolicitacaoDeAmizade\', data: {{idNotificacao: {Id}, codAmizade: {IdCoisa}, n: {{IdUsuarioReceptor: {IdUsuarioTransmissor}, IdUsuarioTransmissor: {IdUsuarioReceptor}, Tipo: 3, IdCoisa: {IdCoisa}, JaViu: false}} }} }}, function(ret) {{tratar(JSON.parte(ret))}});\">Aceitar</button>";
                        btns += $"<button onclick=\"$.post({{url: \'RecusarSolicitacaoDeAmizade\', data: {{idNotificacao: {Id}, codAmizade: {IdCoisa}}} }}, function(ret) {{tratar(JSON.parse(ret))}});\">Recusar</button>";
                        html += $"<li><a href=\"{Link}\">{ToString()}</a>{(JaViu ? "" : btns)}</li>";
                        break;
                    case 5:
                        btns = $"<button onclick=\"$.post({{url: \'/AceitarAdmTarefa\', data: {{codTarefa: {IdCoisa}, idUsuario: {IdUsuarioTransmissor}, codNotif: {Id} }}, async: false }});\">Aceitar</button>";
                        btns += $"<button onclick=\"$.post({{url: \'RecusarAdmEvento\', data: {{codNotif: {Id} }}, async: false }});\">Recusar</button>";
                        html += $"<li><a href=\"{Link}\">{ToString()}</a>{(JaViu ? "" : btns)}</li>";
                        break;
                    case 6:
                        btns = $"<button onclick=\"$.post({{url: \'/AceitarAdmAcontecimento\', data: {{codAcontecimento: {IdCoisa}, idUsuario: {IdUsuarioTransmissor}, codNotif: {Id} }}, async: false }});\">Aceitar</button>";
                        btns += $"<button onclick=\"$.post({{url: \'RecusarAdmEvento\', data: {{codNotif: {Id} }}, async: false }});\">Recusar</button>";
                        html += $"<li><a href=\"{Link}\">{ToString()}</a>{(JaViu ? "" : btns)}</li>";
                        break;
                    case 7:
                        btns = $"<button onclick=\"$.post({{url: \'/AceitarParticipacaoTarefa\', data: {{codTarefa: {IdCoisa}, idUsuario: {IdUsuarioTransmissor}, codNotif: {Id} }}, async: false }});\">Aceitar</button>";
                        btns += $"<button onclick=\"$.post({{url: \'RecusarParticipacaoEvento\', data: {{codNotif: {Id} }}, async: false }});\">Recusar</button>";
                        html += $"<li><a href=\"{Link}\">{ToString()}</a>{(JaViu ? "" : btns)}</li>";
                        break;
                    case 8:
                        btns = $"<button onclick=\"$.post({{url: \'/AceitarParticipacaoAcontecimento\', data: {{codAcontecimento: {IdCoisa}, idUsuario: {IdUsuarioTransmissor}, codNotif: {Id} }}, async: false }});\">Aceitar</button>";
                        btns += $"<button onclick=\"$.post({{url: \'RecusarParticipacaoEvento\', data: {{codNotif: {Id} }}, async: false }});\">Recusar</button>";
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