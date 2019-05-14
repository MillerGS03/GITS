using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace GITS.ViewModel
{
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
                IdUsuariosAdmin = Convert.ToInt16(s["CodUsuarioCriador"]);
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
            IdUsuariosAdmin = codUsuarioCriador;
        }

        public int CodAcontecimento { get; set; }
        public int Tipo { get; set; }
        public int IdUsuariosAdmin { get; set; }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;
            var acontecimento = obj as Acontecimento;
            return acontecimento != null &&
                   base.Equals(obj) &&
                   CodAcontecimento == acontecimento.CodAcontecimento &&
                   Tipo == acontecimento.Tipo &&
                   IdUsuariosAdmin == acontecimento.IdUsuariosAdmin;
        }

        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + CodAcontecimento.GetHashCode();
            hashCode = hashCode * -1521134295 + Tipo.GetHashCode();
            hashCode = hashCode * -1521134295 + IdUsuariosAdmin.GetHashCode();
            return hashCode;
        }
    } //Tipo, Criador
}