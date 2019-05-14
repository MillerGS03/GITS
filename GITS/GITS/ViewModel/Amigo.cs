using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace GITS.ViewModel
{
    public class Amigo
    {
        public Amigo(Usuario u, bool f, bool c)
        {
            Id = u.Id;
            Nome = u.Nome;
            FotoPerfil = u.FotoPerfil;
            XP = u.XP;
            Status = u.Status;
            Insignia = u.Insignia;
            FoiAceito = f;
            ConvidouVoce = c;
        }
        public Amigo()
        {
        }

        public Amigo(int id, string nome, string fotoPerfil, int xP, string status, int insignia, bool aceito, bool convidouVoce)
        {
            Id = id;
            Nome = nome;
            FotoPerfil = fotoPerfil;
            XP = xP;
            Status = status;
            Insignia = insignia;
            FoiAceito = aceito;
            ConvidouVoce = convidouVoce;
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public string FotoPerfil { get; set; }
        public int XP { get; set; }
        public string Status { get; set; }
        public int Insignia { get; set; }
        public bool FoiAceito { get; set; }
        public bool ConvidouVoce { get; set; }

        public override bool Equals(object obj)
        {
            var amigo = obj as Amigo;
            return amigo != null &&
                   Id == amigo.Id &&
                   Nome == amigo.Nome &&
                   FotoPerfil == amigo.FotoPerfil &&
                   XP == amigo.XP &&
                   Status == amigo.Status &&
                   Insignia == amigo.Insignia &&
                   FoiAceito == amigo.FoiAceito &&
                   ConvidouVoce == amigo.ConvidouVoce;
        }

        public override int GetHashCode()
        {
            var hashCode = 321779470;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nome);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FotoPerfil);
            hashCode = hashCode * -1521134295 + XP.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Status);
            hashCode = hashCode * -1521134295 + Insignia.GetHashCode();
            hashCode = hashCode * -1521134295 + FoiAceito.GetHashCode();
            hashCode = hashCode * -1521134295 + ConvidouVoce.GetHashCode();
            return hashCode;
        }
    }
}