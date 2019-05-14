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
                Recompensa = (float)s["Recompensa"];
                GitcoinsObtidos = (float)s["GitcoinsObtidos"];
                TarefasCompletas = Convert.ToInt32(s["GitcoinsObtidos"]);
                
            }
            catch { }
        }
        public Meta()
        {

        }
        public Meta(int codMeta, string titulo, string descricao, string data, int progresso, string ultimaInteracao, float recompensa, float gitcoinsObtidos, int tarefasCompletas)
        {
            CodMeta = codMeta;
            Titulo = titulo;
            Descricao = descricao;
            Data = data;
            Progresso = progresso;
            UltimaInteracao = ultimaInteracao;
            Recompensa = recompensa;
            GitcoinsObtidos = gitcoinsObtidos;
            TarefasCompletas = tarefasCompletas;
        }

        public int CodMeta { get; set; }
        public int Progresso { get; set; }
        public float Recompensa { get; set; }
        public float GitcoinsObtidos { get; set; }
        public int TarefasCompletas { get; set; }
        public string UltimaInteracao { get; set; }
        public List<Tarefa> TarefasRelacionadas { get => Dao.Exec($"select * from Tarefa where CodTarefa in (select CodTarefa from TarefaMeta where CodMeta = {CodMeta}", new List<Tarefa>()); }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;
            var meta = obj as Meta;
            return meta != null &&
                   CodMeta == meta.CodMeta &&
                   Progresso == meta.Progresso &&
                   UltimaInteracao == meta.UltimaInteracao &&
                   Recompensa == meta.Recompensa &&
                   GitcoinsObtidos == meta.GitcoinsObtidos &&
                   TarefasCompletas == meta.TarefasCompletas;
        }

        public override int GetHashCode()
        {
            var hashCode = 1389737090;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + CodMeta.GetHashCode();
            hashCode = hashCode * -1521134295 + Progresso.GetHashCode();
            hashCode = hashCode * -1521134295 + Recompensa.GetHashCode();
            hashCode = hashCode * -1521134295 + GitcoinsObtidos.GetHashCode();
            hashCode = hashCode * -1521134295 + TarefasCompletas.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UltimaInteracao);
            return hashCode;
        }
    } //Progresso, UltimaInteracao
}