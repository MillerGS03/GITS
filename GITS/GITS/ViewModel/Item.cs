﻿using System;
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
                switch(Tipo)
                {
                    case 0:
                        ret = $"<div class=\"tituloUsuario\" style=\"position: relative; {(Conteudo.Length <= 10? "top: 25px; left: 0;" : "top: 35px; left: -10px;")} animation-name: minecraftMenor;\"><span style=\"user-select: none; color: var(--tema); \">{Conteudo}</span></div>";
                        break;
                    case 3:
                        ret = $"<div style=\"background: {Conteudo};\"></div>";
                        break;
                }
                return ret;
            }
        }
        public string ToHtml
        {
            get
            {
                string ret = "";
                switch(Tipo)
                {
                    case 0:
                        ret = $"<iframe scrolling=\"no\" style=\"width: auto; height:90%; position: relative; top: 12em; left: 25%; border:none; transform: scale(2);\" srcdoc=\"<!DOCTYPE html><html><head><meta charset=&quot;UTF-8&quot;><meta name=&quot;viewport&quot; content=&quot;width=device-width, initial-scale=1.0&quot;><meta http-equiv=&quot;X-UA-Compatible&quot; content=&quot;ie=edge&quot;><link rel=&quot;stylesheet&quot; href=&quot;../../Content/materialize.min.css&quot; type=&quot;text/css&quot;><link href=&quot;https://fonts.googleapis.com/icon?family=Material+Icons&quot; rel=&quot;stylesheet&quot;><link rel=&quot;stylesheet&quot; href=&quot;../../Content/index.css&quot; type=&quot;text/css&quot;><link rel=&quot;stylesheet&quot; href=&quot;../../Content/home.css&quot; type=&quot;text/css&quot;></head><body><div class=&quot;imgPerfil hoverable&quot;style=&quot;background:url(imgPerfil) center; background-size:cover;&quot;></div><div class=&quot;tituloUsuario&quot;><span style=&quot;color:var(--tema);&quot;>{Conteudo.Split(' ')[0]}</span></div><style> body {{background: transparent;}} .tituloUsuario{{top: 3em;right: 4em;}}</style></body></html>\"></iframe>";
                        break;
                    case 3:
                        ret = $"<iframe style=\"width: 85%; height: 90%; position: relative; top: 1em;\" srcdoc=\"<!DOCTYPE html><html><head><meta charset=&quot;UTF-8&quot;><meta name=&quot;viewport&quot; content=&quot;width=device-width, initial-scale=1.0&quot;><meta http-equiv=&quot;X-UA-Compatible&quot; content=&quot;ie=edge&quot;><link rel=&quot;stylesheet&quot; href=&quot;../../Content/materialize.min.css&quot; type=&quot;text/css&quot;><link href=&quot;https://fonts.googleapis.com/icon?family=Material+Icons&quot; rel=&quot;stylesheet&quot;><link rel=&quot;stylesheet&quot; href=&quot;../../Content/index.css&quot; type=&quot;text/css&quot;><script src=&quot;../../Scripts/materialize.min.js&quot;></script><link runat=&quot;server&quot; rel=&quot;shortcut icon&quot; href=&quot;favicon.ico&quot; type=&quot;image/x-icon&quot; /><link runat=&quot;server&quot; rel=&quot;icon&quot; href=&quot;favicon.ico&quot; type=&quot;image/ico&quot; /><title>GITS</title></head><body><header><ul id=&quot;notificacoes&quot; class=&quot;dropdown-content&quot;></ul><nav><div class=&quot;nav-wrapper&quot;><a href=&quot;/&quot; class=&quot;brand-logo&quot; id=&quot;titulo&quot;><i class=&quot;material-icons&quot;>hourglass_empty</i>GITS</a><span id=&quot;subtitulo&quot;>Gerenciador de Tempo Sincronizado</span><ul id=&quot;nav-mobile&quot; class=&quot;right hide-on-med-and-down&quot;><li><a class=&quot;dropdown-trigger&quot; href=&quot;#!&quot; data-target=&quot;notificacoes&quot; data-tooltip=&quot;Notificações&quot;><i class=&quot;material-icons&quot; style=&quot;margin-right: 0&quot;>notifications</i></a></li><li><a class=&quot;tooltipped&quot; href=&quot;/faq&quot; data-tooltip=&quot;FAQ&quot;><i class=&quot;material-icons&quot;>question_answer</i></a></li><li><a class=&quot;tooltipped&quot; href=&quot;/bastidores&quot; data-tooltip=&quot;Bastidores&quot;><i class=&quot;material-icons&quot;>code</i></a></li><li><a class=&quot;tooltipped&quot; href=&quot;/&quot; data-tooltip=&quot;Home&quot;><i class=&quot;material-icons&quot;>home</i></a></li></ul><ul id=&quot;slide-out&quot; class=&quot;right sidenav&quot;><li><h3>GITS</h3></li><li><div class=&quot;divider&quot;></div></li><li><a href=&quot;/perfil&quot;><i class=&quot;material-icons&quot;>account_circle</i>Meu perfil</a></li><li><a href=&quot;/&quot;><i class=&quot;material-icons&quot;>home</i>Home</a></li><li><a href=&quot;/bastidores&quot;><i class=&quot;material-icons&quot;>code</i>Bastidores</a></li><li><a href=&quot;/faq&quot;><i class=&quot;material-icons&quot;>question_answer</i>FAQ</a></li><li><a href=&quot;#!&quot;><i class=&quot;material-icons&quot;>notifications</i>Notificações</a></li></ul><a href=&quot;#&quot; data-target=&quot;slide-out&quot; class=&quot;right sidenav-trigger&quot; id=&quot;triggerDireita&quot;><i class=&quot;material-icons&quot;>menu</i></a></div></nav></header><main id=&quot;main&quot;></main><style> body {{background: lightgray;}} nav {{background-color: {Conteudo};}}</style></body></html>\"></iframe>";
                        break;
                        
                }
                return ret;
            }
        }
    }
}