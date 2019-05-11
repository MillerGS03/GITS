using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace GITS.ViewModel
{
    // 0 - Titulo
    // 1 - Decoracao
    // 2 - Insignia
    // 3 - Tema
    public class Item
    {
        public Item()
        {
        }

        public Item(int codItem, string nome, string descricao, double valor, byte tipo, byte metodoObtencao, string conteudo)
        {
            CodItem = codItem;
            Nome = nome;
            Descricao = descricao;
            Valor = valor;
            Tipo = tipo;
            MetodoObtencao = metodoObtencao;
            Conteudo = conteudo;
        }
        public Item(SqlDataReader r)
        {
            try
            {
                CodItem = Convert.ToInt32(r["CodItem"]);
                Nome = r["Nome"].ToString();
                Descricao = r["Descricao"].ToString();
                Valor = Convert.ToDouble(r["Valor"]);
                Tipo = Convert.ToByte(r["Tipo"]);
                MetodoObtencao = Convert.ToByte(r["MetodoObtencao"]);
                Conteudo = r["Conteudo"].ToString();
            }
            catch { }
        }

        public int CodItem { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public double Valor { get; set; }
        public byte Tipo { get; set; }
        public byte MetodoObtencao { get; set; }
        public string Conteudo { get; set; }
        public override bool Equals(object obj)
        {
            var item = obj as Item;
            return item != null &&
                   CodItem == item.CodItem &&
                   Nome == item.Nome &&
                   Descricao == item.Descricao &&
                   Valor == item.Valor &&
                   Tipo == item.Tipo &&
                   MetodoObtencao == item.MetodoObtencao &&
                   Conteudo == item.Conteudo;
        }
        public override int GetHashCode()
        {
            var hashCode = -1575238800;
            hashCode = hashCode * -1521134295 + CodItem.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nome);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Descricao);
            hashCode = hashCode * -1521134295 + Valor.GetHashCode();
            hashCode = hashCode * -1521134295 + Tipo.GetHashCode();
            hashCode = hashCode * -1521134295 + MetodoObtencao.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Conteudo);
            return hashCode;
        }

        public string ToTableHtml
        {
            get
            {
                string ret = "";
                if (Conteudo != null && Conteudo != "")
                    ret += $"<img src=\"{Conteudo}\"><br>";
                ret += Nome;
                return ret;
            }
        }
        public string ToHtml
        {
            get
            {
                return $"<img src=\"{Conteudo}\"><br>{Nome}";
            }
        }
    }
}