using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace GITS.ViewModel
{
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
}