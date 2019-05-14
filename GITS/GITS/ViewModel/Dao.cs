using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GITS.ViewModel
{
    public static class Dao
    {
        public class UsuariosDao
        {
            public class Amizade
            {
                public int CodAmizade { get; set; }
                public int CodUsuario1 { get; set; }
                public int CodUsuario2 { get; set; }
                public bool FoiAceito { get; set; }
                public Amizade() { }
                public Amizade(SqlDataReader s)
                {
                    try
                    {
                        CodAmizade = Convert.ToInt16(s["CodAmizade"]);
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
                        u.Itens = Itens.GetItensDeUsuario(u.Id);
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
                        a.Solicitacoes = Amigos(id, false);
                        a.Tarefas = Eventos.Tarefas(id, true);
                        a.Metas = Eventos.Metas(id);
                        a.Acontecimentos = Eventos.Acontecimentos(id);
                        a.Notificacoes = Notificacoes(id);
                        a.Itens = Itens.GetItensDeUsuario(id);
                        return a;
                    }
                    return null;
                }
                catch { return null; }
            }
            public Usuario[] GetUsuarios(int[] ids)
            {
                try
                {
                    Usuario a;
                    Usuario[] ret = new Usuario[ids.Length];
                    int i = 0;
                    foreach (int id in ids)
                    {
                        a = Exec($"select * from Usuario where Id = {id}", typeof(Usuario));
                        if (a != null && a.Id != 0)
                        {
                            ret[i++] = a;
                        }
                    }
                    return ret;
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
                List<Amizade> l = Exec($"select * from Amizade where (CodUsuario1 = {id} or CodUsuario2 = {id}) and FoiAceito = {(foiAceito ? 1 : 0)}", new List<Amizade>());
                foreach (Amizade a in l)
                {
                    int a1 = a.CodUsuario1;
                    int a2 = a.CodUsuario2;
                    bool aceito = a.FoiAceito;
                    int idAtual = 0;
                    if (a1 == id)
                        idAtual = a2;
                    else if (a2 == id)
                        idAtual = a1;
                    ret.Add(new Amigo((Usuario)Exec($"select * from Usuario where Id = {idAtual}", typeof(Usuario)), aceito, a1 == id));
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
            public void AceitarAmizade(int codAmizade, Notificacao n)
            {
                Amizade s = Exec($"select * from Amizade where CodAmizade = {codAmizade}", typeof(Amizade));
                if (s == null)
                    throw new Exception("Amizade não existe");
                Exec($"update Amizade set FoiAceito = 1 where CodAmizade = {codAmizade}");
                CriarNotificacao(n);
            }
            public int AceitarAmizade(int idAceitou, int idMandou)
            {
                Amizade s = Exec($"select * from Amizade where FoiAceito = 0 and CodUsuario1 = {idMandou} and CodUsuario2 = {idAceitou}", typeof(Amizade));
                if (s == null)
                    throw new Exception("Amizade não existe");
                Exec($"update Amizade set FoiAceito = 1 where CodAmizade = {s.CodAmizade}");
                CriarNotificacao(new Notificacao(idMandou, idAceitou, 3, s.CodAmizade, false));
                return s.CodAmizade;
            }
            public void RecusarAmizade(int codAmizade)
            {
                Amizade s = Exec($"select * from Amizade where CodAmizade = {codAmizade}", typeof(Amizade));
                if (s == null)
                    throw new Exception("Amizade não existe");
                Exec($"delete from Amizade where CodAmizade = {codAmizade}");
            }
            public int RecusarAmizade(int idRecusou, int idMandou)
            {
                Amizade s = Exec($"select * from Amizade where FoiAceito = 0 and CodUsuario1 = {idMandou} and CodUsuario2 = {idRecusou}", typeof(Amizade));
                if (s == null)
                    throw new Exception("Amizade não existe");
                Exec($"delete from Amizade where CodAmizade = {s.CodAmizade}");
                return s.CodAmizade;
            }
            public void CriarNotificacao(Notificacao n)
            {
                Notificacao s = Exec($"select * from Notificacao where Id = {n.Id}", typeof(Notificacao));
                if (s.Id != 0)
                    throw new Exception("Notificacao ja existe");
                Exec($"insert into Notificacao values({n.IdUsuarioReceptor}, {n.IdUsuarioTransmissor}, {n.Tipo}, {n.IdCoisa}, {(n.JaViu ? 1 : 0)})");
                //GitsMessager.EnviarEmail("Notificação", "<h1>NOSSAAAAAAAAAAAAA</h1>" + n.ToHtml, Usuarios.GetUsuario(n.IdUsuarioReceptor).Email);
            }

            public void RemoverNotificacao(int n)
            {
                Notificacao s = Exec($"select * from Notificacao where Id = {n}", typeof(Notificacao));
                if (s.Id != 0)
                    Exec($"delete from Notificacao where Id = {n}");
                else
                    throw new Exception("Notificacao nao existe");
            }
            public void VisualizarNotificacao(int c)
            {
                Notificacao s = Exec($"select * from Notificacao where Id = {c}", typeof(Notificacao));
                if (s.Id != 0)
                    Exec($"update Notificacao set JaViu = 1 where Id = {c}");
                else
                    throw new Exception("Notificacao nao existe");
            }
            public void Publicar(Publicacao publicacao, int[] idsUsuariosMarcados)
            {
                var x = publicacao.Data.ToString();
                publicacao.IdPublicacao = Exec($"publicar_sp {publicacao.IdUsuario}, '{publicacao.Titulo}', '{publicacao.Descricao}', '{publicacao.Data.ToString()}', {publicacao.Likes}, {(publicacao.ComentarioDe == 0 ? "null" : publicacao.ComentarioDe.ToString())}", typeof(int));
                if (idsUsuariosMarcados != null && idsUsuariosMarcados.Length > 0)
                    foreach (int id in idsUsuariosMarcados)
                        Usuarios.CriarNotificacao(new Notificacao(publicacao, id));
            }
            public void RemoverPublicacao(int idPublicacao, int idUsuario)
            {
                Publicacao s = Exec($"select * from Publicacao where CodPublicacao = {idPublicacao} and CodUsuario = {idUsuario}", typeof(Publicacao));
                if (s.IdPublicacao != 0)
                    Exec($"RemoverPublicacao_sp {idPublicacao}");
                else
                    throw new Exception("Publicacao nao existe");
            }
            public void AtualizarPublicacao(int idPublicacao, int idUsuario, string novoTitulo, string novoConteudo)
            {
                Publicacao s = Exec($"select * from Publicacao where CodPublicacao = {idPublicacao} and CodUsuario = {idUsuario}", typeof(Publicacao));
                if (s.IdPublicacao != 0)
                    Exec($"update Publicacao set Titulo='{novoTitulo}', Descricao='{novoConteudo}' where CodPublicacao = {idPublicacao}");
                else
                    throw new Exception("Publicacao nao existe");
            }
            public List<Publicacao> Publicacoes(int idUsuario)
            {
                return Exec($"select * from Publicacao where ComentarioDe is null and CodUsuario = {idUsuario} order by Data desc", new List<Publicacao>());
            }
            public List<Publicacao> PublicacoesRelacionadasA(int idUsuario)
            {
                return Exec($"PublicacoesRelacionadasA_sp {idUsuario}", new List<Publicacao>());
            }
            public Publicacao Publicacao(int idPublicacao)
            {
                return Exec($"select * from Publicacao where CodPublicacao = {idPublicacao}", typeof(Publicacao));
            }
            public Item Tema(int idUsuario)
            {
                return Exec($"select * from Item where CodItem in (select TemaSite from Usuario where Id = {idUsuario})", typeof(Item));
            }

            public void GostarDe(int idPublicacao, int idUsuario)
            {
                Exec($"GostarDe_sp {idUsuario}, {idPublicacao}");
            }
            public void DesgostarDe(int idPublicacao, int idUsuario)
            {
                Exec($"DesgostarDe_sp {idUsuario}, {idPublicacao}");
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
                t.CodTarefa = Exec($"adicionarTarefa {t.Urgencia}, '{t.Data}', '{t.Titulo}', '{t.Descricao}', {t.Dificuldade}, {t.IdUsuariosAdmin}, {(t.Meta == null ? 0 : t.Meta.CodMeta)}", typeof(int));
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
                Exec($"insert into Acontecimento values({a.Tipo}, '{a.Data}', '{a.Titulo}', '{a.Descricao}', {a.IdUsuariosAdmin})");
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
                Usuarios.CriarNotificacao(new Notificacao(idUsuario, s.IdUsuariosAdmin, 0, s.CodTarefa, false));
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
                {
                    t.Meta = Exec($"select * from Meta where CodMeta in (select CodMeta from TarefaMeta where CodTarefa = {t.CodTarefa})", typeof(Meta));
                    t.IdUsuariosMarcados = (Exec($"select * from UsuarioTarefa where CodTarefa = {t.CodTarefa}", typeof(List<int>)));
                }
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
        public class ItensDao
        {
            public ItensDao()
            {
            }
            public List<Item> GetItensDeUsuario(int id)
            {
                return Exec($"select * from Item where CodItem in (select CodItem from UsuarioItem where IdUsuario = {id})", new List<Item>());
            }
            public List<Item> GetItensDeTipo(byte tipo)
            {
                return Exec($"select * from Item where Tipo = {tipo}", new List<Item>());
            }
            public List<Item> GetItensDeTipoEUsuario(byte tipo, int id)
            {
                return Exec($"select * from Item where Tipo = {tipo} and CodItem in (select CodItem from UsuarioItem where IdUsuario = {id})", new List<Item>());
            }
            public List<Item> GetTodosItens()
            {
                return Exec($"select * from Item", new List<Item>());
            }
            public Item GetItem(int id)
            {
                return Exec($"select * from Item where CodItem = {id}", typeof(Item));
            }
            public void Comprar(int idUsuario, int idItem)
            {
                Exec($"comprarItem_sp {idUsuario}, {idItem}");
            }
            public void TrocarTitulo(int t, int idUsuario)
            {
                string tituloAtual = Usuarios.GetUsuario(idUsuario).Titulo;
                string[] partesTitulo = tituloAtual.Split(' ');
                partesTitulo[0] = t.ToString();
                string tituloNovo = string.Join(" ", partesTitulo);
                Exec($"update Usuario set Titulo = '{tituloNovo}' where Id = {idUsuario}");
            }
            public void TrocarTitulo(string e, int idUsuario)
            {
                string tituloAtual = Usuarios.GetUsuario(idUsuario).Titulo;
                Exec($"update Usuario set Titulo = '{tituloAtual + " " + e}' where Id = {idUsuario}");
            }
            public void TirarEfeitoDeTitulo(string e, int idUsuario)
            {
                string[] tituloAtual = Usuarios.GetUsuario(idUsuario).Titulo.Split(' ');
                tituloAtual[Array.IndexOf(tituloAtual, e)] = "";
                Exec($"update Usuario set Titulo = '{string.Join(" ", tituloAtual)}' where Id = {idUsuario}");
            }
        }

        private const string conexaoBD = "Data Source = regulus.cotuca.unicamp.br; Initial Catalog =PR118179;User ID =PR118179;Password=MillerScherer1;Min Pool Size=5;Max Pool Size=250;MultipleActiveResultSets=true;";
        private static SqlConnection conexao = new SqlConnection(conexaoBD);
        private static SqlCommand comando;
        public static UsuariosDao Usuarios
        {
            get
            {
                return new UsuariosDao();
            }
        }
        public static ItensDao Itens
        {
            get
            {
                return new ItensDao();
            }
        }
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
                if (conexao.State != System.Data.ConnectionState.Open)
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
        public static dynamic Exec(string command, IList lista, bool fechar)
        {
            SqlDataReader ret = null;
            try
            {
                comando = new SqlCommand(command, conexao);
                if (conexao.State != System.Data.ConnectionState.Open)
                    conexao.Open();
                ret = comando.ExecuteReader();
                while (ret != null && ret.Read())
                    lista.Add(lista.GetType().GetGenericArguments()[0].GetConstructor(new Type[] { typeof(SqlDataReader) }).Invoke(new object[] { ret }));
                if (fechar)
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
                if (conexao.State != System.Data.ConnectionState.Open)
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
            catch (Exception ex) { return null; }
        }
        public static void Exec(string command)
        {
            SqlDataReader ret = null;
            try
            {
                comando = new SqlCommand(command, conexao);
                if (conexao.State != System.Data.ConnectionState.Open)
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