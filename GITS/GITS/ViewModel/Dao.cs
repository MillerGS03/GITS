using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using static GITS.ViewModel.Usuario;

namespace GITS.ViewModel
{
    public static class Dao
    {
        public class UsuariosDao
        {
            public class Amizade
            {
                public int CodUsuario1 { get; set; }
                public int CodUsuario2 { get; set; }
                public bool FoiAceito { get; set; }
                public Amizade() { }
                public Amizade(SqlDataReader s)
                {
                    CodUsuario1 = Convert.ToInt16(s["CodUsuario1"]);
                    CodUsuario2 = Convert.ToInt16(s["CodUsuario2"]);
                    FoiAceito   = Convert.ToByte(s["FoiAceito"]) == 1;
                }
            }
            public UsuariosDao()
            {
            }
            public List<Usuario> ToList()
            {
                List<Usuario> usuarios = new List<Usuario>();
                usuarios = Exec("select * from Usuario", usuarios);
                foreach (Usuario u in usuarios)
                {
                    u.Amigos = Amigos(u.Id);
                    u.Tarefas = Eventos.Tarefas(u.Id);
                    u.Metas = Eventos.Metas(u.Id);
                    u.Acontecimentos = Eventos.Acontecimentos(u.Id);
                }
                return usuarios;
            }
            public int Add(Usuario u)
            {
                Usuario user = Exec($"select * from Usuario where Id = {u.Id}", typeof(Usuario));
                if (user == null)
                    Exec($"insert into Usuario values('{u.CodUsuario}', '{u.Email}', '{u.FotoPerfil}', {u.XP}, '{u.Status}', {u.Insignia}, '{u.Titulo}', {u.Decoracao}, {u.TemaSite}, {u.Dinheiro}, '{u.Nome}')");
                var retornoId = Exec($"select Id from Usuario where CodUsuario = {u.CodUsuario}", typeof(Usuario));

                if (retornoId.Id > 0)
                    return retornoId.Id;
                else
                    throw new Exception("Erro na inserção!");
            }
            public void Remove(Usuario u)
            {
                Usuario s = Exec($"select * from Usuario where Id = {u.Id}", typeof(Usuario));
                if (s != null)
                    Exec($"removerUsuario {u.Id}");
            }
            public void Remove(int id)
            {
                Usuario s = Exec($"select * from Usuario where Id = {id}", typeof(Usuario));
                if (s != null)
                    Exec($"removerUsuario {id}");
            }
            public void Update(Usuario u)
            {
                Usuario s = Exec($"select * from Usuario where Id = {u.Id}", typeof(Usuario));
                if (s != null)
                    Exec($"update Usuario set Nome = '{u.Nome}', FotoPerfil = '{u.FotoPerfil}', Email = '{u.Email}', Decoracao = {u.Decoracao}, TemaSite = {u.TemaSite}, Dinheiro = {u.Dinheiro}, Titulo = '{u.Titulo}', _Status = '{u.Status}', XP = {u.XP} where Id = {u.Id}");
            }
            public List<Amigo> Amigos(int id)
            {
                List<Amigo> ret = new List<Amigo>();
                List<object[]> idsAmigos = new List<object[]>();
                List<Amizade> l = Exec($"select * from Amizade where CodUsuario1 = {id} or CodUsuario2 = {id}", new List<Amizade>());
                foreach (Amizade a in l)
                {
                    int a1 = a.CodUsuario1;
                    int a2 = a.CodUsuario2;
                    bool aceito = a.FoiAceito;
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
                    Usuario s = (Usuario)Exec($"select * from Usuario where Id = {(int)i[0]}", typeof(Usuario));
                    ret.Add(new Amigo(s, (bool)i[1]));
                }
                return ret;
            }
            public void CriarAmizade(int um, int dois)
            {
                Amizade s = Exec($"select * from Amizade where (CodUsuario1 = {um} or CodUsuario2 = {um}) and (CodUsuario1 = {dois} or CodUsuario2 = {dois})", typeof(Amizade));
                if (s != null)
                    throw new Exception("Amizade ja existe");
                Exec($"insert into Amizade values({um}, {dois}, 0)");
            }
        }
        public class EventosDao
        {
            public EventosDao()
            {
            }
            public void CriarTarefa(Tarefa t)
            {
                Tarefa s = Exec($"select * from Tarefa where CodTarefa = {t.CodTarefa}", typeof(Tarefa));
                if (s != null)
                    throw new Exception("Tarefa ja existe");
                Exec($"insert into Tarefa values({t.Urgencia}, '{t.Data}', '{t.Titulo}', '{t.Descricao}', {t.Dificuldade})");
            }
            public void RemoverTarefa(Tarefa t)
            {
                Tarefa s = Exec($"select * from Tarefa where CodTarefa = {t.CodTarefa}", typeof(Tarefa));
                if (s != null)
                    Exec($"delete from Tarefa where CodTarefa = {t.CodTarefa}");
                throw new Exception("Tarefa nao existe");
            }
            public void CriarAcontecimento(Acontecimento a)
            {
                Acontecimento s = Exec($"select * from Acontecimento where CodAcontecimento = {a.CodAcontecimento}", typeof(Acontecimento));
                if (s != null)
                    throw new Exception("Acontecimento ja existe");
                Exec($"insert into Acontecimento values({a.Tipo}, '{a.Data}', '{a.Titulo}', '{a.Descricao}', {a.CodUsuarioCriador})");
            }
            public void RemoverAcontecimento(Acontecimento a)
            {
                Acontecimento s = Exec($"select * from Acontecimento where CodAcontecimento = {a.CodAcontecimento}", typeof(Acontecimento));
                if (s != null)
                    Exec($"delete from Acontecimento where CodAcontecimento = {a.CodAcontecimento}");
                throw new Exception("Acontecimento nao existe");
            }
            public void AdicionarUsuarioATarefa(Usuario u, int codTarefa)
            {
                Tarefa s = Exec($"select * from Tarefa where CodTarefa = {codTarefa}", typeof(Tarefa));
                if (s != null)
                    Exec($"insert into UsuarioTarefa values({u.Id}, {codTarefa})");
                else
                    throw new Exception("Tarefa invalida");
            }
            public void RemoverUsuarioDeTarefa(Usuario u, int codTarefa)
            {
                Tarefa s = Exec($"select * from UsuarioTarefa where CodTarefa = {codTarefa} and IdUsuario = {u.Id}", typeof(Tarefa));
                if (s != null)
                    Exec($"delete from UsuarioTarefa where CodTarefa = {codTarefa} and IdUsuario = {u.Id}");
                else
                    throw new Exception("Tarefa ou usuario invalido");
            }
            public void AdicionarUsuarioAAcontecimento(Usuario u, int codAcontecimento)
            {
                Acontecimento s = Exec($"select * from Acontecimento where CodAcontecimento = {codAcontecimento}", typeof(Acontecimento));
                if (s != null)
                    Exec($"insert into AcontecimentoUsuario values({codAcontecimento}, {u.Id})");
                else
                    throw new Exception("Acontecimento invalido!");
            }
            public void RemoverUsuarioDeAcontecimento(Usuario u, int codAcontecimento)
            {
                Acontecimento s = Exec($"select * from AcontecimentoUsuario where CodAcontecimento = {codAcontecimento} and CodUsuario = {u.Id}", typeof(Acontecimento));
                if (s != null)
                    Exec($"delete from AcontecimentoUsuario where CodAcontecimento = {codAcontecimento} and CodUsuario = {u.Id}");
                else
                    throw new Exception("Usuario ou acontecimento invalido");
            }
            public List<Tarefa> Tarefas(int id)
            {
                List<Tarefa> lista = new List<Tarefa>();
                lista = Exec($"select * from Tarefa where CodTarefa in(select CodTarefa from UsuarioTarefa where IdUsuario = {id})", lista);
                foreach(Tarefa t in lista)
                    t.Meta = Exec($"select * from Meta where CodMeta in (select CodMeta from TarefaMeta where CodTarefa = {t.CodTarefa})", typeof(Meta));
                return lista;
            }
            public List<Tarefa> Tarefas(Meta meta)
            {
                List<Tarefa> lista = new List<Tarefa>();
                lista = Exec($"select * from Tarefa where CodTarefa in(select CodTarefa from TarefaMeta where CodMeta = {meta.CodMeta})", lista);
                foreach(Tarefa t in lista)
                    t.Meta = meta;
                return lista;
            }
            public List<Meta> Metas(int id)
            {
                return Exec($"select * from Meta where CodMeta in (select CodMeta from UsuarioMeta where IdUsuario = {id})", new List<Meta>());
            }
            public List<Acontecimento> Acontecimentos(int id)
            {
                return Exec($"select * from Acontecimento where CodAcontecimento in (select CodAcontecimento from AcontecimentoUsuario where CodUsuario = {id})", new List<Acontecimento>());
            }
            public Tarefa Tarefa(int id)
            {
                Tarefa ret = Exec($"select * from Tarefa where CodTarefa = {id}", typeof(Tarefa));
                if (ret != null)
                    ret.Meta = Exec($"select * from Meta where CodMeta in (select CodMeta from TarefaMeta where CodTarefa = {ret.CodTarefa})", typeof(Meta));
                return ret;
            }
            public Meta Meta(int id)
            {
                return Exec($"select * from Meta where CodMeta = {id}", typeof(Meta));
            }
        }
        //public class ItensDao : Dao { }

        private const string conexaoBD = "Data Source = regulus.cotuca.unicamp.br; Initial Catalog =PR118179;User ID =PR118179;Password=MillerScherer1;Min Pool Size=5;Max Pool Size=250;";
        private static SqlConnection conexao = new SqlConnection(conexaoBD);
        private static SqlCommand comando;
        public static UsuariosDao Usuarios
        {
            get
            {
                return new UsuariosDao();
            }
        }
        //public static ItensDao Itens
        //{
        //    get
        //    {
        //        return new ItensDao();
        //    }
        //}
        public static EventosDao Eventos
        {
            get
            {
                return new EventosDao();
            }
        }
        public static dynamic Exec(string command, IList lista)
        {
            SqlDataReader ret = null;
            try
            {
                comando = new SqlCommand(command, conexao);
                conexao.Open();
                ret = comando.ExecuteReader();
                while (ret != null && ret.Read())
                    lista.Add(lista.GetType().GetGenericArguments()[0].GetConstructor(new Type[] { typeof(SqlDataReader) }).Invoke(new object[] { ret }));
                conexao.Close();
                return lista;
            }
            catch (Exception e) { return null; }
        }
        public static dynamic Exec(string command, Type tipo)
        {
            SqlDataReader ret = null;
            try
            {
                comando = new SqlCommand(command, conexao);
                conexao.Open();
                ret = comando.ExecuteReader();
                var t = tipo.GetConstructor(new Type[] { }).Invoke(new object[] { });
                if (ret != null && ret.Read())
                    t = tipo.GetConstructor(new Type[] { typeof(SqlDataReader) }).Invoke(new object[] { ret });
                conexao.Close();
                return t;
            }
            catch (Exception e) { return null; }
        }
        public static void Exec(string command)
        {
            SqlDataReader ret = null;
            try
            {
                comando = new SqlCommand(command, conexao);
                conexao.Open();
                ret = comando.ExecuteReader();
                conexao.Close();
            }
            catch (Exception e) { throw e; }
        }
        //public static SqlDataReader Exec(string command)
        //{
        //    SqlDataReader ret = null;
        //    try
        //    {
        //        comando = new SqlCommand(command, conexao);
        //        conexao.Open();
        //        ret = comando.ExecuteReader();
        //        return ret;
        //    }
        //    catch (Exception e) { return ret; }
        //}
    }
}