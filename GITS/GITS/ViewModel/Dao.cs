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
                    try
                    {
                        CodUsuario1 = Convert.ToInt16(s["CodUsuario1"]);
                        CodUsuario2 = Convert.ToInt16(s["CodUsuario2"]);
                        FoiAceito = Convert.ToByte(s["FoiAceito"]) == 1;
                    }
                    catch { }
                }
            }
            public UsuariosDao()
            {
            }
            public List<Usuario> ToList()
            {
                List<Usuario> usuarios = new List<Usuario>();
                usuarios = Exec("select * from Usuario", usuarios);
                if (usuarios != null)
                {
                    foreach (Usuario u in usuarios)
                    {
                        u.Amigos = Amigos(u.Id, true);
                        u.Tarefas = Eventos.Tarefas(u.Id, true);
                        u.Metas = Eventos.Metas(u.Id);
                        u.Acontecimentos = Eventos.Acontecimentos(u.Id);
                        u.Notificacoes = Notificacoes(u.Id);
                    }
                }
                return usuarios;
            }
            public Usuario GetUsuario(int id)
            {
                try
                {
                    Usuario a = Exec($"select * from Usuario where Id = {id}", typeof(Usuario));
                    if (a != null && a.Id != 0)
                    {
                        a.Amigos = Amigos(id, true);
                        a.Tarefas = Eventos.Tarefas(id, true);
                        a.Metas = Eventos.Metas(id);
                        a.Acontecimentos = Eventos.Acontecimentos(id);
                        a.Notificacoes = Notificacoes(id);
                        return a;
                    }
                    return null;
                }
                catch { return null; }
            }
            public int Add(Usuario u)
            {
                Usuario user = Exec($"select * from Usuario where Id = {u.Id}", typeof(Usuario));
                if (user.Id == 0)
                    Exec($"insert into Usuario values('{u.CodUsuario}', '{u.Email}', '{u.FotoPerfil}', {u.XP}, '{u.Status}', {u.Insignia}, '{u.Titulo}', {u.Decoracao}, {u.TemaSite}, {u.Dinheiro}, '{u.Nome}')");
                string x = $"insert into Usuario values('{u.CodUsuario}', '{u.Email}', '{u.FotoPerfil}', {u.XP}, '{u.Status}', {u.Insignia}, '{u.Titulo}', {u.Decoracao}, {u.TemaSite}, {u.Dinheiro}, '{u.Nome}')";
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
            public List<Amigo> Amigos(int id, bool foiAceito)
            {
                List<Amigo> ret = new List<Amigo>();
                List<object[]> idsAmigos = new List<object[]>();
                List<Amizade> l = Exec($"select * from Amizade where (CodUsuario1 = {id} or CodUsuario2 = {id}) and FoiAceito = {(foiAceito ? 1 : 0)}", new List<Amizade>());
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
            public List<Notificacao> Notificacoes(int id)
            {
                List<Notificacao> ns = Exec($"select * from Notificacao where IdUsuarioReceptor = {id}", new List<Notificacao>());
                return ns;
            }
            public void CriarAmizade(int um, int dois)
            {
                Amizade s = Exec($"select * from Amizade where (CodUsuario1 = {um} or CodUsuario2 = {um}) and (CodUsuario1 = {dois} or CodUsuario2 = {dois})", typeof(Amizade));
                if (s.CodUsuario1 != 0)
                    throw new Exception("Amizade ja existe");
                Exec($"insert into Amizade values({um}, {dois}, 0)");
                CriarNotificacao(new Notificacao(dois, um, 1, Exec($"select CodAmizade from Amizade where (CodUsuario1 = {um} or CodUsuario2 = {um}) and (CodUsuario1 = {dois} or CodUsuario2 = {dois})", typeof(int)), false));
            }
            public void RemoverAmizade(int um, int dois)
            {
                Amizade s = Exec($"select * from Amizade where (CodUsuario1 = {um} or CodUsuario2 = {um}) and (CodUsuario1 = {dois} or CodUsuario2 = {dois})", typeof(Amizade));
                if (s.CodUsuario1 == 0)
                    throw new Exception("Amizade nao existe");
                Exec($"delete from Amizade where (CodUsuario1 = {um} or CodUsuario2 = {um}) and (CodUsuario1 = {dois} or CodUsuario2 = {dois})");
            }
            public void CriarNotificacao(Notificacao n)
            {
                Notificacao s = Exec($"select * from Notificacao where Id = {n.Id}", typeof(Notificacao));
                if (s.Id != 0)
                    throw new Exception("Notificacao ja existe");
                Exec($"insert into Notificacao values({n.IdUsuarioReceptor}, {n.IdUsuarioTransmissor}, {n.Tipo}, {n.IdCoisa}, {(n.JaViu ? 1 : 0)})");
            }
            public void RemoverNotificacao(int n)
            {
                Notificacao s = Exec($"select * from Notificacao where Id = {n}", typeof(Notificacao));
                if (s.Id != 0)
                    Exec($"delete from Notificacao where CodNotificacao = {n}");
                else
                    throw new Exception("Notificacao nao existe");
            }
            public void Publicar(Publicacao publicacao, int[] idsUsuariosMarcados)
            {
                Exec($"insert into Publicacao values({publicacao.IdUsuario}, {publicacao.Titulo}, {publicacao.Descricao}, {publicacao.DataFormatada})");
                if (idsUsuariosMarcados != null && idsUsuariosMarcados.Length > 0)
                    foreach (int id in idsUsuariosMarcados)
                    {
                        Notificacao not = new Notificacao(id, publicacao.IdUsuario, 2, publicacao.IdPublicacao, false);
                        Usuarios.CriarNotificacao(not);
                    }
            }
        }
        public class EventosDao
        {
            public EventosDao()
            {
            }
            public void CriarTarefa(ref Tarefa t)
            {
                Tarefa s = Exec($"select * from Tarefa where CodTarefa = {t.CodTarefa}", typeof(Tarefa));
                if (s.CodTarefa != 0)
                    throw new Exception("Tarefa ja existe");
                t.CodTarefa = Exec($"adicionarTarefa {t.Urgencia}, '{t.Data}', '{t.Titulo}', '{t.Descricao}', {t.Dificuldade}, {t.CodUsuarioCriador}, {(t.Meta == null ? 0 : t.Meta.CodMeta)}", typeof(int));
            }
            public void RemoverTarefa(int t)
            {
                Tarefa s = Exec($"select * from Tarefa where CodTarefa = {t}", typeof(Tarefa));
                if (s.CodTarefa != 0)
                    Exec($"delete from Tarefa where CodTarefa = {t}");
                throw new Exception("Tarefa nao existe");
            }
            public void CriarAcontecimento(Acontecimento a)
            {
                Acontecimento s = Exec($"select * from Acontecimento where CodAcontecimento = {a.CodAcontecimento}", typeof(Acontecimento));
                if (s.CodAcontecimento != 0)
                    throw new Exception("Acontecimento ja existe");
                Exec($"insert into Acontecimento values({a.Tipo}, '{a.Data}', '{a.Titulo}', '{a.Descricao}', {a.CodUsuarioCriador})");
            }
            public void RemoverAcontecimento(int a)
            {
                Acontecimento s = Exec($"select * from Acontecimento where CodAcontecimento = {a}", typeof(Acontecimento));
                if (s.CodAcontecimento != 0)
                    Exec($"delete from Acontecimento where CodAcontecimento = {a}");
                else
                    throw new Exception("Acontecimento nao existe");
            }
            public void AdicionarUsuarioATarefa(int idUsuario, int codTarefa)
            {
                Tarefa s = Exec($"select * from Tarefa where CodTarefa = {codTarefa}", typeof(Tarefa));
                if (s.CodTarefa == 0)
                    throw new Exception("Tarefa invalida");
                Exec($"insert into UsuarioTarefa values({idUsuario}, {codTarefa}, 0)");
                Usuarios.CriarNotificacao(new Notificacao(idUsuario, s.CodUsuarioCriador, 0, s.CodTarefa, false));
            }
            public void RemoverUsuarioDeTarefa(int idUsuario, int codTarefa)
            {
                Tarefa s = Exec($"select * from UsuarioTarefa where CodTarefa = {codTarefa} and IdUsuario = {idUsuario}", typeof(Tarefa));
                if (s.CodTarefa != 0)
                    Exec($"delete from UsuarioTarefa where CodTarefa = {codTarefa} and IdUsuario = {idUsuario}");
                else
                    throw new Exception("Tarefa ou usuario invalido");
            }
            public void AdicionarUsuarioAAcontecimento(int idUsuario, int codAcontecimento)
            {
                Acontecimento s = Exec($"select * from Acontecimento where CodAcontecimento = {codAcontecimento}", typeof(Acontecimento));
                if (s.CodAcontecimento != 0)
                    Exec($"insert into AcontecimentoUsuario values({codAcontecimento}, {idUsuario})");
                else
                    throw new Exception("Acontecimento invalido!");
            }
            public void RemoverUsuarioDeAcontecimento(int idUsuario, int codAcontecimento)
            {
                Acontecimento s = Exec($"select * from AcontecimentoUsuario where CodAcontecimento = {codAcontecimento} and CodUsuario = {idUsuario}", typeof(Acontecimento));
                if (s.CodAcontecimento != 0)
                    Exec($"delete from AcontecimentoUsuario where CodAcontecimento = {codAcontecimento} and CodUsuario = {idUsuario}");
                else
                    throw new Exception("Usuario ou acontecimento invalido");
            }
            public List<Tarefa> Tarefas(int id, bool aceita)
            {
                List<Tarefa> lista = new List<Tarefa>();
                lista = Exec($"select * from Tarefa where CodTarefa in(select CodTarefa from UsuarioTarefa where IdUsuario = {id} and FoiAceita = {(aceita ? 1 : 0)})", lista);
                foreach (Tarefa t in lista)
                    t.Meta = Exec($"select * from Meta where CodMeta in (select CodMeta from TarefaMeta where CodTarefa = {t.CodTarefa})", typeof(Meta));
                return lista;
            }
            public List<Tarefa> Tarefas(Meta meta)
            {
                List<Tarefa> lista = new List<Tarefa>();
                lista = Exec($"select * from Tarefa where CodTarefa in(select CodTarefa from TarefaMeta where CodMeta = {meta.CodMeta})", lista);
                foreach (Tarefa t in lista)
                    t.Meta = meta;
                return lista;
            }
            public List<Meta> Metas(int id)
            {
                var l = Exec($"select * from Meta where CodMeta in (select CodMeta from UsuarioMeta where IdUsuario = {id})", new List<Meta>());
                if (l != null)
                    return l;
                throw new Exception();
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
                if (ret != null)
                    return lista;
                throw new Exception();
            }
            catch { return null; }
        }
        public static dynamic Exec(string command, Type tipo)
        {
            SqlDataReader ret = null;
            try
            {
                comando = new SqlCommand(command, conexao);
                conexao.Open();
                ret = comando.ExecuteReader();
                dynamic t;
                if (tipo == typeof(int))
                {
                    ret.Read();
                    t = Convert.ToInt32(ret.GetValue(0));
                    conexao.Close();
                    return t;
                }
                t = tipo.GetConstructor(new Type[] { }).Invoke(new object[] { });
                if (ret != null && ret.Read() && tipo != typeof(int))
                    t = tipo.GetConstructor(new Type[] { typeof(SqlDataReader) }).Invoke(new object[] { ret });
                conexao.Close();
                if (t != null)
                    return t;
                throw new Exception();
            }
            catch { return null; }
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