using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using static GITS.ViewModel.Usuario;

namespace GITS.ViewModel
{
    public class Dao : Controller
    {
        public class UsuariosDao : Dao
        {
            public UsuariosDao()
            {
            }
            public List<Usuario> ToList()
            {
                List<Usuario> usuarios = new List<Usuario>();
                SqlDataReader dr = Exec("select * from Usuario");
                while (dr.Read())
                    usuarios.Add(new Usuario(dr));
                foreach (Usuario u in usuarios)
                {
                    u.Amigos = Amigos(u.Id);
                    u.Tarefas = new EventosDao().Tarefas(u.Id);
                    u.Metas = new EventosDao().Metas(u.Id);
                    u.Acontecimentos = new EventosDao().Acontecimentos(u.Id);
                }
                return usuarios;
            }
            public int Add(Usuario u)
            {
                SqlDataReader s = Exec($"select * from Usuario where Id = {u.Id}");
                if ((s == null || !s.Read()) && ModelState.IsValid)
                    Exec($"insert into Usuario values('{u.CodUsuario}', '{u.Email}', '{u.FotoPerfil}', {u.XP}, '{u.Status}', {u.Insignia}, '{u.Titulo}', {u.Decoracao}, {u.TemaSite}, {u.Dinheiro}, '{u.Nome}')");
                var retornoId = Exec($"select Id from Usuario where CodUsuario = {u.CodUsuario}");

                if (retornoId.Read())
                    return Convert.ToInt16(retornoId["Id"]);
                else
                    throw new Exception("Erro na inserção!");
            }
            public void Remove(Usuario u)
            {
                SqlDataReader s = Exec($"select * from Usuario where Id = {u.Id}");
                if (s != null && s.Read())
                    Exec($"removerUsuario {u.Id}");
            }
            public void Remove(int id)
            {
                SqlDataReader s = Exec($"select * from Usuario where Id = {id}");
                if (s != null && s.Read())
                    Exec($"removerUsuario {id}");
            }
            public void Update(Usuario u)
            {
                SqlDataReader s = Exec($"select * from Usuario where Id = {u.Id}");
                if (s != null && s.Read())
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
                    SqlDataReader s = Exec($"select * from Usuario where Id = {(int)i[0]}");
                    s.Read();
                    var atual = new Usuario(s);
                    ret.Add(new Amigo(atual.Id, atual.Nome, atual.FotoPerfil, atual.XP, atual.Status, atual.Insignia, (bool)i[1]));
                }
                return ret;
            }
            public void CriarAmizade(int um, int dois)
            {
                SqlDataReader s = Exec($"select * from Amizade where (CodUsuario1 = {um} or CodUsuario2 = {um}) and (CodUsuario1 = {dois} or CodUsuario2 = {dois})");
                if (s != null && s.Read())
                    throw new Exception("Amizade ja existe");
                Exec($"insert into Amizade values({um}, {dois}, 0)");
            }
        }
        public class EventosDao : Dao
        {
            public EventosDao()
            {
            }
            public void CriarTarefa(Tarefa t)
            {
                SqlDataReader s = Exec($"select * from Tarefa where CodTarefa = {t.CodTarefa}");
                if (s != null && s.Read())
                    throw new Exception("Tarefa ja existe");
                Exec($"insert into Tarefa values({t.Urgencia}, '{t.Data}', '{t.Titulo}', '{t.Descricao}', {t.Dificuldade})");
            }
            public void RemoverTarefa(Tarefa t)
            {
                SqlDataReader s = Exec($"select * from Tarefa where CodTarefa = {t.CodTarefa}");
                if (s != null && s.Read())
                    Exec($"delete from Tarefa where CodTarefa = {t.CodTarefa}");
                throw new Exception("Tarefa nao existe");
            }
            public void CriarAcontecimento(Acontecimento a)
            {
                SqlDataReader s = Exec($"select * from Acontecimento where CodAcontecimento = {a.CodAcontecimento}");
                if (s != null && s.Read())
                    throw new Exception("Acontecimento ja existe");
                Exec($"insert into Acontecimento values({a.Tipo}, '{a.Data}', '{a.Titulo}', '{a.Descricao}', {a.CodUsuarioCriador})");
            }
            public void RemoverAcontecimento(Acontecimento a)
            {
                SqlDataReader s = Exec($"select * from Acontecimento where CodAcontecimento = {a.CodAcontecimento}");
                if (s != null && s.Read())
                    Exec($"delete from Acontecimento where CodAcontecimento = {a.CodAcontecimento}");
                throw new Exception("Acontecimento nao existe");
            }
            public void AdicionarUsuarioATarefa(Usuario u, int codTarefa)
            {
                SqlDataReader s = Exec($"select * from Tarefa where CodTarefa = {codTarefa}");
                if (s != null && s.Read())
                    Exec($"insert into UsuarioTarefa values({u.Id}, {codTarefa})");
                else
                    throw new Exception("Tarefa invalida");
            }
            public void RemoverUsuarioDeTarefa(Usuario u, int codTarefa)
            {
                SqlDataReader s = Exec($"select * from UsuarioTarefa where CodTarefa = {codTarefa} and IdUsuario = {u.Id}");
                if (s != null && s.Read())
                    Exec($"delete from UsuarioTarefa where CodTarefa = {codTarefa} and IdUsuario = {u.Id}");
                else
                    throw new Exception("Tarefa ou usuario invalido");
            }
            public void AdicionarUsuarioAAcontecimento(Usuario u, int codAcontecimento)
            {
                SqlDataReader s = Exec($"select * from Acontecimento where CodAcontecimento = {codAcontecimento}");
                if (s != null && s.Read())
                    Exec($"insert into AcontecimentoUsuario values({codAcontecimento}, {u.Id})");
                else
                    throw new Exception("Acontecimento invalido!");
            }
            public void RemoverUsuarioDeAcontecimento(Usuario u, int codAcontecimento)
            {
                SqlDataReader s = Exec($"select * from AcontecimentoUsuario where CodAcontecimento = {codAcontecimento} and CodUsuario = {u.Id}");
                if (s != null && s.Read())
                    Exec($"delete from AcontecimentoUsuario where CodAcontecimento = {codAcontecimento} and CodUsuario = {u.Id}");
                else
                    throw new Exception("Usuario ou acontecimento invalido");
            }
            public List<Tarefa> Tarefas(int id)
            {
                List<Tarefa> lista = new List<Tarefa>();
                SqlDataReader dr = Exec($"select * from Tarefa where CodTarefa in(select CodTarefa from UsuarioTarefa where IdUsuario = {id})");
                while (dr != null && dr.Read())
                {
                    Meta meta = null;
                    int codTarefa = Convert.ToInt16(dr["CodTarefa"]);
                    SqlDataReader dr2 = Exec($"select * from Meta where CodMeta in (select CodMeta from TarefaMeta where CodTarefa = {codTarefa})");
                    if (dr2 != null && dr2.Read())
                        meta = new Meta(dr2);
                    var tarefa = new Tarefa(dr);
                    tarefa.Meta = meta;
                    lista.Add(tarefa);
                }
                return lista;
            }
            public List<Tarefa> Tarefas(Meta meta)
            {
                List<Tarefa> lista = new List<Tarefa>();
                SqlDataReader dr = Exec($"select * from Tarefa where CodTarefa in(select CodTarefa from TarefaMeta where CodMeta = {meta.CodMeta})");
                while (dr != null && dr.Read())
                {
                    var tarefa = new Tarefa(dr);
                    tarefa.Meta = meta;
                    lista.Add(tarefa);
                }
                return lista;
            }
            public List<Meta> Metas(int id)
            {
                List<Meta> lista = new List<Meta>();
                SqlDataReader dr = Exec($"select * from Meta where CodMeta in (select CodMeta from UsuarioMeta where IdUsuario = {id})");
                while (dr != null && dr.Read())
                    lista.Add(new Meta(dr));
                return lista;
            }
            public List<Acontecimento> Acontecimentos(int id)
            {
                List<Acontecimento> lista = new List<Acontecimento>();
                SqlDataReader dr = Exec($"select * from Acontecimento where CodAcontecimento in (select CodAcontecimento from AcontecimentoUsuario where CodUsuario = {id})");
                while (dr != null && dr.Read())
                    lista.Add(new Acontecimento(dr));
                return lista;
            }
            public Tarefa Tarefa(int id)
            {
                Tarefa ret = null;
                SqlDataReader s = Exec($"select * from Tarefa where CodTarefa = {id}");
                if (s != null && s.Read())
                {
                    Meta meta = null;
                    ret = new Tarefa(s);
                    s = Exec($"select * from Meta where CodMeta in (select CodMeta from TarefaMeta where CodTarefa = {ret.CodTarefa})");
                    if (s != null && s.Read())
                        meta = new Meta(s);
                    ret.Meta = meta;
                }
                return ret;
            }
            public Meta Meta(int id)
            {
                Meta ret = null;
                SqlDataReader s = Exec($"select * from Meta where CodMeta = {id}");
                if (s != null && s.Read())
                    ret = new Meta(s);
                return ret;
            }
        }
        public class ItensDao : Dao { }

        private const string conexaoBD = "Data Source = regulus.cotuca.unicamp.br; Initial Catalog =PR118179;User ID =PR118179;Password=MillerScherer1;Min Pool Size=5;Max Pool Size=250;";
        private SqlConnection conexao;
        private SqlCommand comando;

        public Dao()
        {
            conexao = new SqlConnection(conexaoBD);
            conexao.Open();
        }
        public void Fechar()
        {
            conexao.Close();
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
        public EventosDao Eventos
        {
            get
            {
                return new EventosDao();
            }
        }
        protected SqlDataReader Exec(string command)
        {
            SqlDataReader ret = null;
            try
            {

                comando = new SqlCommand(command, conexao);
              //  conexao.Open();
                ret = comando.ExecuteReader();
                return ret;
            }
            catch (Exception e) { return ret; }
        }
    }
}