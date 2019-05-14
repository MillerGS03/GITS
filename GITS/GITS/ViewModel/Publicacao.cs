using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace GITS.ViewModel
{
    public class Publicacao
    {
        public class Like
        {
            public Like(SqlDataReader s)
            {
                CodLike = Convert.ToInt32(s["CodGostei"]);
                IdPublicacao = Convert.ToInt32(s["IdPublicacao"]);
                IdUsuario = Convert.ToInt32(s["IdUsuario"]);
            }
            public Like(int codLike, int idPublicacao, int idUsuario)
            {
                CodLike = codLike;
                IdPublicacao = idPublicacao;
                IdUsuario = idUsuario;
            }

            public int CodLike { get; set; }
            public int IdPublicacao { get; set; }
            public int IdUsuario { get; set; }

            public override bool Equals(object obj)
            {
                return obj is Like like &&
                       CodLike == like.CodLike &&
                       IdPublicacao == like.IdPublicacao &&
                       IdUsuario == like.IdUsuario;
            }

            public override int GetHashCode()
            {
                var hashCode = -919713977;
                hashCode = hashCode * -1521134295 + CodLike.GetHashCode();
                hashCode = hashCode * -1521134295 + IdPublicacao.GetHashCode();
                hashCode = hashCode * -1521134295 + IdUsuario.GetHashCode();
                return hashCode;
            }
        }
        public Publicacao(SqlDataReader s)
        {
            IdPublicacao = Convert.ToInt32(s["CodPublicacao"]);
            IdUsuario = Convert.ToInt32(s["CodUsuario"]);
            Titulo = s["Titulo"].ToString();
            Descricao = s["Descricao"].ToString();
            Data = (DateTime)s["Data"];
            Likes = Convert.ToInt32(s["Likes"]);

            try
            {
                ComentarioDe = Convert.ToInt32(s["ComentarioDe"]);
            }
            catch { } // Não é comentário

            Comentarios = Dao.Exec($"select * from Publicacao where ComentarioDe = {IdPublicacao}", new List<Publicacao>(), false);
        }
        public Publicacao(int idUsuario, string titulo, string descricao, DateTime data, int likes, int? comentarioDe)
        {
            IdUsuario = idUsuario;
            Titulo = titulo;
            Descricao = descricao;
            Data = data;
            Likes = likes;
            ComentarioDe = comentarioDe == null ? 0 : comentarioDe.Value;
            Comentarios = new List<Publicacao>();
        }

        public Publicacao(int idPublicacao, int idUsuario, string titulo, string descricao, DateTime data, int likes, int? comentarioDe)
        {
            IdPublicacao = idPublicacao;
            IdUsuario = idUsuario;
            Titulo = titulo;
            Descricao = descricao;
            Data = data;
            Likes = likes;
            ComentarioDe = comentarioDe == null ? 0 : comentarioDe.Value;
            Comentarios = Dao.Exec($"select * from Publicacao where ComentarioDe = {IdPublicacao}", new List<Publicacao>());
        }
        public Publicacao() { }

        public int IdPublicacao { get; set; }
        public int IdUsuario { get; set; }
        public Usuario Usuario { get => Dao.Usuarios.GetUsuario(IdUsuario); }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public int Likes { get; set; }
        public bool Gostou(int idUsuario)
        {
            return Dao.Exec($"select * from Gostei where IdUsuario = {idUsuario} and IdPublicacao = {IdPublicacao}", new List<Like>()).Count > 0;
        }
        public int ComentarioDe { get; set; }
        public List<Publicacao> Comentarios { get; set; }
        public List<Usuario> UsuariosMarcados
        {

            get
            {
                var lista = new List<Usuario>();
                return Dao.Exec($"select * from Usuario where Id in (select IdUsuarioReceptor from Notificacao where Tipo=2 and IdCoisa in (select CodPublicacao from Publicacao where CodPublicacao = ${IdPublicacao}))", lista);
            }
        }

    }
}