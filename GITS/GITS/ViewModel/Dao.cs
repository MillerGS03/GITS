using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace GITS.ViewModel
{
    public class Dao
    {
        public class UsuariosDao : Dao
        {
            protected List<Usuario> usuarios;

            public UsuariosDao()
            {
                usuarios = new List<Usuario>();
                SqlDataReader dr = Exec("select * from Usuario");
                while (dr.Read())
                {
                    Usuario user = new Usuario(
                    Convert.ToInt16(dr["Id"]),
                    dr["CodUsuario"].ToString(),
                    dr["Email"].ToString(),
                    dr["Nome"].ToString(),
                    dr["FotoPerfil"].ToString(),
                    Convert.ToInt16(dr["XP"]),
                    dr["_Status"].ToString(),
                    Convert.ToInt16(dr["Insignia"]),
                    Convert.ToDouble(dr["Dinheiro"]),
                    dr["Titulo"].ToString(),
                    Convert.ToInt16(dr["TemaSite"]),
                    Convert.ToInt16(dr["Decoracao"]));
                    usuarios.Add(user);
                }
                foreach(Usuario u in usuarios)
                {
                    List<int> idsAmigos = new List<int>();
                    dr = Exec($"select * from Amizade where CodUsuario1 = {u.Id}");
                    while (dr != null && dr.Read())
                        idsAmigos.Add(Convert.ToInt32(dr["CodUsuario2"]));
                    u.Amigos = new List<Usuario>();
                    foreach (int id in idsAmigos)
                        u.Amigos.Add(usuarios.Find(us => us.Id == id));
                }
            }
            public List<Usuario> ToList()
            {
                return usuarios;
            }
            public void Add(Usuario u)
            {
                if (usuarios.Find(usuario => usuario.CodUsuario.Equals(u.CodUsuario)) == null)
                    Exec($"insert into Usuario values('{u.CodUsuario}', '{u.Email}', '{u.FotoPerfil}', {u.XP}, '{u.Status}', {u.Insignia}, '{u.Titulo}', {u.Decoracao}, {u.TemaSite}, {u.Dinheiro}, '{u.Nome}')");
            }
            public void Remove(Usuario u)
            {
                if (usuarios.Find(us => us.Id == u.Id) != null)
                {
                    string exclusao = "";
                    // excluir de todas as tabelas
                    exclusao += $"delete from Usuario where Id = {u.Id}";
                    Exec(exclusao);
                }
            } //nao esta pronto
            public void Remove(int id)
            {
                if (usuarios.Find(us => us.Id == id) != null)
                {
                    string exclusao = "";
                    // excluir de todas as tabelas
                    exclusao += $"delete from Usuario where Id = {id}";
                    Exec(exclusao);
                }
            } //nao esta pronto
            public void Remove(string cod)
            {
                if (usuarios.Find(us => us.CodUsuario.Equals(cod)) != null)
                {
                    string exclusao = "";
                    // excluir de todas as tabelas
                    exclusao += $"delete from Usuario where CodUsuario = '{cod}'";
                    Exec(exclusao);
                }
            } //nao esta pronto
            public void Update(Usuario u)
            {
                if (usuarios.Find(us => us.Id == u.Id) != null)
                    Exec($"update Usuario set Nome = '{u.Nome}', FotoPerfil = '{u.FotoPerfil}', Email = '{u.Email}', Decoracao = {u.Decoracao}, TemaSite = {u.TemaSite}, Dinheiro = {u.Dinheiro}, Titulo = '{u.Titulo}', _Status = '{u.Status}', XP = {u.XP} where Id = {u.Id}");
            }
            public List<Usuario> Amigos(int id)
            {
                List<Usuario> ret = new List<Usuario>();
                List<int> idsAmigos = new List<int>();
                SqlDataReader dr = Exec($"select * from Amizade where CodUsuario1 = {id}");
                while (dr != null && dr.Read())
                    idsAmigos.Add(Convert.ToInt32(dr["CodUsuario2"]));
                foreach (int i in idsAmigos)
                    ret.Add(usuarios.Find(us => us.Id == id));
                return ret;
            }
        }
        public class ItensDao : Dao { }

        private const string conexaoBD = "Data Source = regulus.cotuca.unicamp.br; Initial Catalog =PR118179;User ID =PR118179;Password=MillerScherer1";
        private SqlConnection conexao;
        private SqlCommand comando;
        public Dao()
        {
            conexao = new SqlConnection(conexaoBD);
            comando = new SqlCommand("", conexao);
            conexao.Open();
        }

        public UsuariosDao Usuarios
        {
            get
            {
                return new UsuariosDao();
            }
        }
        public ItensDao Itens
        {
            get
            {
                return new ItensDao();
            }
        }
        protected SqlDataReader Exec(string command)
        {
            SqlDataReader ret = null;
            try
            {
                conexao = new SqlConnection(conexaoBD);
                comando = new SqlCommand(command, conexao);
                conexao.Open();
                ret = comando.ExecuteReader();
                return ret;
            }
            catch (Exception e) { return ret; }
        }
    }
}