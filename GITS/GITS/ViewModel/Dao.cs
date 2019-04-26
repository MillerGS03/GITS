using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using static GITS.ViewModel.Usuario;

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
                foreach (Usuario u in usuarios)
                {
                    u.Amigos = Amigos(u.Id);
                    u.Tarefas = Tarefas(u.Id);
                }
            }
            public List<Usuario> ToList()
            {
                return usuarios;
            }
            public int Add(Usuario u)
            {
                if (usuarios.Find(usuario => usuario.CodUsuario.Equals(u.CodUsuario)) == null)
                    Exec($"insert into Usuario values('{u.CodUsuario}', '{u.Email}', '{u.FotoPerfil}', {u.XP}, '{u.Status}', {u.Insignia}, '{u.Titulo}', {u.Decoracao}, {u.TemaSite}, {u.Dinheiro}, '{u.Nome}')");
                var retornoId = Exec($"select Id from Usuario where CodUsuario = {u.CodUsuario}");

                if (retornoId.Read())
                    return Convert.ToInt16(retornoId["Id"]);
                else
                    throw new Exception("Erro na inserção!");
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
            public List<Amigo> Amigos(int id)
            {
                List<Amigo> ret = new List<Amigo>();
                List<object[]> idsAmigos = new List<object[]>();
                SqlDataReader dr = Exec($"select * from Amizade where CodUsuario1 = {id} or CodUsuario2 = {id}");
                while (dr != null && dr.Read())
                {
                    int a1 = Convert.ToInt32(dr["CodUsuario1"]);
                    int a2 = Convert.ToInt32(dr["CodUsuario2"]);
                    bool aceito = Convert.ToByte(dr["FoiAceito"]) == 1;
                    object[] atual = new object[2];
                    if (a1 == id)
                        atual[0] = a2;
                    else if (a2 == id)
                        atual[0] = a1;
                    atual[1] = aceito;
                    idsAmigos.Add(atual);
                }
                foreach (object[] i in idsAmigos)
                {
                    var atual = usuarios.Find(us => us.Id == (int)i[0]);
                    ret.Add(new Amigo(atual.Id, atual.Nome, atual.FotoPerfil, atual.XP, atual.Status, atual.Insignia, (bool)i[1]));
                }
                return ret;
            }
            public List<Tarefa> Tarefas(int id)
            {
                List<Tarefa> lista = new List<Tarefa>();
                lista.Add(new Tarefa("Lista de física", "Descrição da tarefa em questão", 3, 2, DateTime.Now));
                return lista;
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