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
        }
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
                IdUsuariosAdmin = Convert.ToInt16(s["CodUsuarioCriador"]);
            }
            catch { }
        }
        public Tarefa(int codTarefa, string titulo, string descricao, int dificuldade, int urgencia, string data, Meta meta, int codUsuarioCriador, List<int> marcados)
        {
            CodTarefa = codTarefa;
            Titulo = titulo;
            Descricao = descricao;
            Dificuldade = dificuldade;
            Urgencia = urgencia;
            Data = data;
            Meta = meta;
            IdUsuariosAdmin = codUsuarioCriador;
            IdUsuariosMarcados = marcados;
        }
        public Tarefa(string titulo, string descricao, int dificuldade, int urgencia, string data, Meta meta, int codUsuarioCriador, List<int> marcados)
        {
            Titulo = titulo;
            Descricao = descricao;
            Dificuldade = dificuldade;
            Urgencia = urgencia;
            Data = data;
            Meta = meta;
            IdUsuariosAdmin = codUsuarioCriador;
            IdUsuariosMarcados = marcados;
        }
        public Tarefa(string titulo, string descricao, int dificuldade, int urgencia, string data)
        {
            Titulo = titulo;
            Descricao = descricao;
            Dificuldade = dificuldade;
            Urgencia = urgencia;
            Data = data;
            IdUsuariosMarcados = new List<int>();
        }

        public int CodTarefa { get; set; }
        public int Dificuldade { get; set; }
        public int Urgencia { get; set; }
        public Meta Meta { get; set; }
        public int IdUsuariosAdmin { get; set; }
        public List<int> IdUsuariosMarcados { get; set; }

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
                   IdUsuariosAdmin == tarefa.IdUsuariosAdmin &&
                   EqualityComparer<Meta>.Default.Equals(Meta, tarefa.Meta);
        }

        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + CodTarefa.GetHashCode();
            hashCode = hashCode * -1521134295 + Dificuldade.GetHashCode();
            hashCode = hashCode * -1521134295 + Urgencia.GetHashCode();
            hashCode = hashCode * -1521134295 + IdUsuariosAdmin.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Meta>.Default.GetHashCode(Meta);
            return hashCode;
        }
    }  //Dificuldade, Urgencia, Criador, Meta
}