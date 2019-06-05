using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace GITS.ViewModel
{
    public class EmailConfig
    {
        public EmailConfig(SqlDataReader s)
        {
            RelatorioDiario = Convert.ToByte(s["RelatorioDiario"]) == 1;
            RequisicoesAdministracao = Convert.ToByte(s["RequisicoesAdministracao"]) == 1;
            PedidosAmizade = Convert.ToByte(s["PedidosAmizade"]) == 1;
            NotificacoesAmizadesAceitas = Convert.ToByte(s["NotificacoesAmizadesAceitas"]) == 1;
            RequisicoesEntrar = Convert.ToByte(s["RequisicoesEntrar"]) == 1;
            AvisosSaida = Convert.ToByte(s["AvisosSaida"]) == 1;
            TornouSeAdm = Convert.ToByte(s["TornouSeAdm"]) == 1;
            ConviteTarefaAcontecimento = Convert.ToByte(s["ConviteTarefaAcontecimento"]) == 1;
            MarcadoEmPost = Convert.ToByte(s["MarcadoEmPost"]) == 1;
            DataUltimoRelatorioEnviado = (DateTime)s["DataUltimoRelatorioEnviado"];
        }
        public EmailConfig()
        {
            DataUltimoRelatorioEnviado = new DateTime();
        }

        public EmailConfig(bool relatorioDiario, bool requisicoesAdministracao, bool pedidosAmizade, bool notificacoesAmizadesAceitas, bool requisicoesEntrar, bool avisosSaida, bool tornouSeAdm, bool conviteTarefaAcontecimento, bool marcadoEmPost, DateTime dataUltimoRelatorioEnviado)
        {
            RelatorioDiario = relatorioDiario;
            RequisicoesAdministracao = requisicoesAdministracao;
            PedidosAmizade = pedidosAmizade;
            NotificacoesAmizadesAceitas = notificacoesAmizadesAceitas;
            RequisicoesEntrar = requisicoesEntrar;
            AvisosSaida = avisosSaida;
            TornouSeAdm = tornouSeAdm;
            ConviteTarefaAcontecimento = conviteTarefaAcontecimento;
            MarcadoEmPost = marcadoEmPost;
            DataUltimoRelatorioEnviado = dataUltimoRelatorioEnviado;
        }

        public bool RelatorioDiario { get; set; }
        public bool RequisicoesAdministracao { get; set; }
        public bool PedidosAmizade { get; set; }
        public bool NotificacoesAmizadesAceitas { get; set; }
        public bool RequisicoesEntrar { get; set; }
        public bool AvisosSaida { get; set; }
        public bool TornouSeAdm { get; set; }
        public bool ConviteTarefaAcontecimento { get; set; }
        public bool MarcadoEmPost { get; set; }
        public DateTime DataUltimoRelatorioEnviado { get; set; }

        public override bool Equals(object obj)
        {
            var config = obj as EmailConfig;
            return config != null &&
                   RelatorioDiario == config.RelatorioDiario &&
                   RequisicoesAdministracao == config.RequisicoesAdministracao &&
                   PedidosAmizade == config.PedidosAmizade &&
                   NotificacoesAmizadesAceitas == config.NotificacoesAmizadesAceitas &&
                   RequisicoesEntrar == config.RequisicoesEntrar &&
                   AvisosSaida == config.AvisosSaida &&
                   TornouSeAdm == config.TornouSeAdm &&
                   ConviteTarefaAcontecimento == config.ConviteTarefaAcontecimento &&
                   MarcadoEmPost == config.MarcadoEmPost &&
                   DataUltimoRelatorioEnviado == config.DataUltimoRelatorioEnviado;
        }

        public override int GetHashCode()
        {
            var hashCode = -1806068098;
            hashCode = hashCode * -1521134295 + RelatorioDiario.GetHashCode();
            hashCode = hashCode * -1521134295 + RequisicoesAdministracao.GetHashCode();
            hashCode = hashCode * -1521134295 + PedidosAmizade.GetHashCode();
            hashCode = hashCode * -1521134295 + NotificacoesAmizadesAceitas.GetHashCode();
            hashCode = hashCode * -1521134295 + RequisicoesEntrar.GetHashCode();
            hashCode = hashCode * -1521134295 + AvisosSaida.GetHashCode();
            hashCode = hashCode * -1521134295 + TornouSeAdm.GetHashCode();
            hashCode = hashCode * -1521134295 + ConviteTarefaAcontecimento.GetHashCode();
            hashCode = hashCode * -1521134295 + MarcadoEmPost.GetHashCode();
            hashCode = hashCode * -1521134295 + DataUltimoRelatorioEnviado.GetHashCode();
            return hashCode;
        }
    }
}