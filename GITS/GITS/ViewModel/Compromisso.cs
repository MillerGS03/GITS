using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace GITS.ViewModel
{
    public class Compromisso
    {
        public string Data { get; set; }
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
}