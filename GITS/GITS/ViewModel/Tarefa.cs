using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace GITS.ViewModel
{
    public class Tarefa : Compromisso
    {
        public Tarefa()
        {
            IdUsuariosMarcados = new List<int>();
            IdUsuariosAdmin = new List<int>();
        }
        public Tarefa(SqlDataReader s) : this()
        {
            try
            {
                CodTarefa = Convert.ToInt16(s["CodTarefa"]);
                Titulo = s["Titulo"].ToString();
                Descricao = s["Descricao"].ToString();
                Dificuldade = Convert.ToInt16(s["Dificuldade"]);
                Data = s["Data"].ToString().Substring(0, 10);
                Meta = null;
                IdUsuariosAdmin.Add(Convert.ToInt16(s["CodUsuarioCriador"]));
                Recompensa = Convert.ToInt32(s["Recompensa"]);
                Criacao = s["Criacao"].ToString().Substring(0, 10);
            }
            catch { }
        }
        public Tarefa(int codTarefa, string titulo, string descricao, int dificuldade, int urgencia, string data, Meta meta, int codUsuarioCriador, List<int> marcados, int recomp, string criacao) : this()
        {
            CodTarefa = codTarefa;
            Titulo = titulo;
            Descricao = descricao;
            Dificuldade = dificuldade;
            Urgencia = urgencia;
            Data = data;
            Meta = meta;
            IdUsuariosAdmin.Add(codUsuarioCriador);
            IdUsuariosMarcados = marcados;
            Recompensa = recomp;
            Criacao = criacao;
        }
        public Tarefa(string titulo, string descricao, int dificuldade, int urgencia, string data, Meta meta, int codUsuarioCriador, List<int> marcados, int recomp, string criacao) : this()
        {
            Titulo = titulo;
            Descricao = descricao;
            Dificuldade = dificuldade;
            Urgencia = urgencia;
            Data = data;
            Meta = meta;
            IdUsuariosAdmin.Add(codUsuarioCriador);
            IdUsuariosMarcados = marcados;
            Recompensa = recomp;
            Criacao = criacao;
        }
        public Tarefa(string titulo, string descricao, int dificuldade, int urgencia, string data, int recomp, string criacao) : this()
        {
            Titulo = titulo;
            Descricao = descricao;
            Dificuldade = dificuldade;
            Urgencia = urgencia;
            Data = data;
            Recompensa = recomp;
            Criacao = criacao;
        }

        public int CodTarefa { get; set; }
        public int Dificuldade { get; set; }
        public int Urgencia { get; set; }
        public Meta Meta { get; set; }
        public List<int> IdUsuariosAdmin { get; set; }
        public List<int> IdUsuariosMarcados { get; set; }
        public int Recompensa { get; set; }
        public string Criacao { get; set; }

        public override bool Equals(object obj)
        {
            var tarefa = obj as Tarefa;
            return tarefa != null &&
                   base.Equals(obj) &&
                   CodTarefa == tarefa.CodTarefa &&
                   Dificuldade == tarefa.Dificuldade &&
                   Urgencia == tarefa.Urgencia &&
                   EqualityComparer<Meta>.Default.Equals(Meta, tarefa.Meta) &&
                   EqualityComparer<List<int>>.Default.Equals(IdUsuariosAdmin, tarefa.IdUsuariosAdmin) &&
                   EqualityComparer<List<int>>.Default.Equals(IdUsuariosMarcados, tarefa.IdUsuariosMarcados) &&
                   Recompensa == tarefa.Recompensa &&
                   Criacao == tarefa.Criacao;
        }

        public override int GetHashCode()
        {
            var hashCode = 91076492;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + CodTarefa.GetHashCode();
            hashCode = hashCode * -1521134295 + Dificuldade.GetHashCode();
            hashCode = hashCode * -1521134295 + Urgencia.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Meta>.Default.GetHashCode(Meta);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<int>>.Default.GetHashCode(IdUsuariosAdmin);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<int>>.Default.GetHashCode(IdUsuariosMarcados);
            hashCode = hashCode * -1521134295 + Recompensa.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Criacao);
            return hashCode;
        }
    }  //Dificuldade, Urgencia, Criador, Meta
}