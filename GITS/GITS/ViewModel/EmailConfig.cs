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
            AdministracaoTarefa = Convert.ToByte(s["AdministracaoTarefa"]) == 1;
            DataUltimoRelatorioEnviado = (DateTime)s["DataUltimoRelatorioEnviado"];
        }
        public EmailConfig()
        {
            RelatorioDiario = true;
            RequisicoesAdministracao = false;
            PedidosAmizade = false;
            NotificacoesAmizadesAceitas = false;
            AdministracaoTarefa = false;
            DataUltimoRelatorioEnviado = new DateTime();
        }
        public EmailConfig(bool relatorioDiario, bool requisicoesAdministracao, bool pedidosAmizade, bool notificacoesAmizadesAceitas, bool administracaoTarefa, DateTime dataUltimoRelatorioEnviado)
        {
            RelatorioDiario = relatorioDiario;
            RequisicoesAdministracao = requisicoesAdministracao;
            PedidosAmizade = pedidosAmizade;
            NotificacoesAmizadesAceitas = notificacoesAmizadesAceitas;
            DataUltimoRelatorioEnviado = dataUltimoRelatorioEnviado;
            AdministracaoTarefa = administracaoTarefa;
        }

        public bool RelatorioDiario { get; set; }
        public bool RequisicoesAdministracao { get; set; }
        public bool PedidosAmizade { get; set; }
        public bool NotificacoesAmizadesAceitas { get; set; }
        public bool AdministracaoTarefa { get; set; }
        public DateTime DataUltimoRelatorioEnviado { get; set; }

        public override bool Equals(object obj)
        {
            return obj is EmailConfig config &&
                   RelatorioDiario == config.RelatorioDiario &&
                   RequisicoesAdministracao == config.RequisicoesAdministracao &&
                   PedidosAmizade == config.PedidosAmizade &&
                   NotificacoesAmizadesAceitas == config.NotificacoesAmizadesAceitas &&
                   DataUltimoRelatorioEnviado == config.DataUltimoRelatorioEnviado &&
                   AdministracaoTarefa == config.AdministracaoTarefa;
        }

        public override int GetHashCode()
        {
            var hashCode = -95378070;
            hashCode = hashCode * -1521134295 + RelatorioDiario.GetHashCode();
            hashCode = hashCode * -1521134295 + RequisicoesAdministracao.GetHashCode();
            hashCode = hashCode * -1521134295 + PedidosAmizade.GetHashCode();
            hashCode = hashCode * -1521134295 + NotificacoesAmizadesAceitas.GetHashCode();
            hashCode = hashCode * -1521134295 + DataUltimoRelatorioEnviado.GetHashCode();
            hashCode = hashCode * -1521134295 + AdministracaoTarefa.GetHashCode();
            return hashCode;
        }
    }
}