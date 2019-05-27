﻿using System;
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
                    Exec($"AdicionarUsuario_sp '{u.CodUsuario}', '{u.Email}', '{u.FotoPerfil}', '{u.Nome}'");
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
            public List<Notificacao> Notificacoes(string constraint)
            {
                return Exec($"select * from Notificacao where {constraint}", new List<Notificacao>());
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
            public Item Decoracao(int idUsuario)
            {
                return Exec($"select * from Item where CodItem in (select Decoracao from Usuario where Id = {idUsuario})", typeof(Item));
            }
            public Item Insignia(int idUsuario)
            {
                return Exec($"select * from Item where CodItem in (select Insignia from Usuario where Id = {idUsuario})", typeof(Item));
            }
            public string Titulo(int idUsuario)
            {
                return Exec($"select Titulo from Usuario where Id = {idUsuario}", typeof(string));
            }

            public void GostarDe(int idPublicacao, int idUsuario)
            {
                Exec($"GostarDe_sp {idUsuario}, {idPublicacao}");
            }
            public void DesgostarDe(int idPublicacao, int idUsuario)
            {
                Exec($"DesgostarDe_sp {idUsuario}, {idPublicacao}");
            }
            public void AdicionarMeta(Meta meta, int idCriador)
            {
                Exec($"insert into Meta values ('{meta.Titulo}', '{meta.Descricao}', {(meta.Data == null ? "null" : $"'{meta.Data}'")}, {meta.Progresso}, '{meta.UltimaInteracao}', {meta.Recompensa}, {meta.GitcoinsObtidos}, {meta.TarefasCompletas})");
                var idMeta = Exec($"select max(CodMeta) from Meta", typeof(int));
                Exec($"insert into UsuarioMeta values ({idCriador}, {idMeta})");
            }
            public void AtualizarMeta(Meta meta, int idCriador)
            {
                if (Exec($"select count(*) from Meta where CodMeta={meta.CodMeta} and CodMeta in (select CodMeta from UsuarioMeta where IdUsuario={idCriador})", typeof(int)) == 0)
                    throw new Exception("A meta não existe!");

                Exec($"update Meta set Titulo='{meta.Titulo}', Descricao='{meta.Descricao}', Data={(meta.Data == null ? "null" : $"'{meta.Data}'")}, Recompensa={meta.Recompensa} where CodMeta={meta.CodMeta}");
            }
            public void RemoverMeta(int idCriador, int idMeta)
            {
                if (Exec($"select count(*) from Meta where CodMeta={idMeta} and CodMeta in (select CodMeta from UsuarioMeta where IdUsuario={idCriador})", typeof(int)) == 0)
                    throw new Exception("A meta não existe!");

                Exec($"delete from UsuarioMeta where CodMeta={idMeta}");
                Exec($"delete from Meta where CodMeta={idMeta}");
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
                t.CodTarefa = Exec($"adicionarTarefa '{t.Data}', '{t.Titulo}', '{t.Descricao}', {t.Dificuldade}, {t.IdUsuariosAdmin[0]}, {(t.Meta == null ? 0 : t.Meta.CodMeta)}, {t.Recompensa}, '{t.Data}', {t.XP}", typeof(int));
            }
            public void RemoverTarefa(int t)
            {
                Tarefa s = Exec($"select * from Tarefa where CodTarefa = {t}", typeof(Tarefa));
                if (s.CodTarefa != 0)
                {
                    Exec($"delete from UsuarioTarefa where CodTarefa = {t}");
                    Exec($"delete from AdminTarefa where CodTarefa = {t}");
                    Exec($"delete from TarefaMeta where CodTarefa = {t}");
                    Exec($"delete from Tarefa where CodTarefa = {t}");
                    Exec($"delete from Notificacao where IdCoisa = {t} and Tipo = 2");
                }
                else
                    throw new Exception("Tarefa nao existe");
            }
            public void UpdateTarefaFull(Tarefa t, int idMetaAnterior = 0)
            {
                Exec($"update Tarefa set Data = '{t.Data}', Titulo = '{t.Titulo}', Descricao = '{t.Descricao}', Dificuldade = {t.Dificuldade}, Recompensa = {t.Recompensa}, XP = {t.XP} where CodTarefa = {t.CodTarefa}");
                if (t.Meta != null && idMetaAnterior != 0)
                    Exec($"update TarefaMeta set CodMeta = {t.Meta.CodMeta} where CodTarefa = {t.CodTarefa} and CodMeta = {idMetaAnterior}");
                else if (t.Meta != null)
                    Exec($"insert into TarefaMeta values({t.CodTarefa}, {t.Meta.CodMeta})");
                else
                {
                    Meta antiga = Eventos.Meta(idMetaAnterior);
                    if (antiga.CodMeta != 0)
                        Exec($"delete from TarefaMeta where CodTarefa = {t.CodTarefa} and CodMeta = {idMetaAnterior}");
                }
            }
            public void UpdateTarefa(Tarefa t, int idMetaAnterior = 0)
            {
                if (t.Meta != null && idMetaAnterior != 0)
                    Exec($"update TarefaMeta set CodMeta = {t.Meta.CodMeta} where CodTarefa = {t.CodTarefa} and CodMeta = {idMetaAnterior}");
                else if (t.Meta != null)
                    Exec($"insert into TarefaMeta values({t.CodTarefa}, {t.Meta.CodMeta})");
                else
                {
                    Meta antiga = Eventos.Meta(idMetaAnterior);
                    if (antiga.CodMeta != 0)
                        Exec($"delete from TarefaMeta where CodTarefa = {t.CodTarefa} and CodMeta = {idMetaAnterior}");
                }
            }
            public void UpdateAcontecimento(Acontecimento a)
            {
                Exec($"update Acontecimento set Titulo = '{a.Titulo}', Descricao = '{a.Descricao}', Tipo = {a.Tipo}, Data = '{a.Data}'");
            }
            public void RequisitarAdminTarefa(int t, int u)
            {
                Usuarios.CriarNotificacao(new Notificacao(Tarefa(t).IdUsuariosAdmin[0], u, 4, t, false));
            }
            public void AdicionarAdminATarefa(int t, int u)
            {
                Exec($"insert into AdminTarefa values({u}, {t})");
            }
            public void RequisitarAdminAcontecimento(int a, int u)
            {
                Usuarios.CriarNotificacao(new Notificacao(Acontecimento(a).IdUsuariosAdmin[0], u, 4, a, false));
            }
            public void AdicionarAdminAAcontecimento(int a, int u)
            {
                Exec($"insert into AdminAcontecimento values({u}, {a})");
            }
            public void CriarAcontecimento(ref Acontecimento a)
            {
                Acontecimento s = Exec($"select * from Acontecimento where CodAcontecimento = {a.CodAcontecimento}", typeof(Acontecimento));
                if (s.CodAcontecimento != 0)
                    throw new Exception("Acontecimento ja existe");
                a.CodAcontecimento = Exec($"adicionarAcontecimento_sp '{a.Titulo}', '{a.Descricao}', '{a.Data}', {a.Tipo}, {a.IdUsuariosAdmin[0]}", typeof(int));
            }
            public void RemoverAcontecimento(int a)
            {
                Acontecimento s = Exec($"select * from Acontecimento where CodAcontecimento = {a}", typeof(Acontecimento));
                if (s.CodAcontecimento != 0)
                {
                    Exec($"delete from AdminAcontecimento where CodAcontecimento = {a}");
                    Exec($"delete from UsuarioAcontecimento where CodAcontecimento = {a}");
                    Exec($"delete from Acontecimento where CodAcontecimento = {a}");
                }
                else
                    throw new Exception("Acontecimento nao existe");
            }
            public void AdicionarUsuarioATarefa(int idUsuario, int codTarefa)
            {
                Tarefa s = Exec($"select * from Tarefa where CodTarefa = {codTarefa}", typeof(Tarefa));
                if (s.CodTarefa == 0)
                    throw new Exception("Tarefa invalida");
                Exec($"insert into UsuarioTarefa values({idUsuario}, {codTarefa}, 0, 0)");
                Usuarios.CriarNotificacao(new Notificacao(idUsuario, s.IdUsuariosAdmin[0], 0, s.CodTarefa, false));
            }
            public void RemoverUsuarioDeTarefa(int idUsuario, int codTarefa)
            {
                Tarefa s = Exec($"select * from UsuarioTarefa where CodTarefa = {codTarefa} and IdUsuario = {idUsuario}", typeof(Tarefa));
                if (s.CodTarefa != 0)
                {
                    if (s.IdUsuariosAdmin.Contains(idUsuario))
                        Exec($"delete from AdminTarefa where CodTarefa = {codTarefa}");
                    Exec($"delete from UsuarioTarefa where CodTarefa = {codTarefa} and IdUsuario = {idUsuario}");
                }
                else
                    throw new Exception("Tarefa ou usuario invalido");
            }
            public void AdicionarUsuarioAAcontecimento(int idUsuario, int codAcontecimento)
            {
                Acontecimento s = Exec($"select * from Acontecimento where CodAcontecimento = {codAcontecimento}", typeof(Acontecimento));
                if (s.CodAcontecimento != 0)
                    Exec($"insert into UsuarioAcontecimento values({idUsuario}, {codAcontecimento})");
                else
                    throw new Exception("Acontecimento invalido!");
            }
            public void RemoverUsuarioDeAcontecimento(int idUsuario, int codAcontecimento)
            {
                Acontecimento s = Exec($"select * from AcontecimentoUsuario where CodAcontecimento = {codAcontecimento} and CodUsuario = {idUsuario}", typeof(Acontecimento));
                if (s.CodAcontecimento != 0)
                {
                    Exec($"delete from AdminAcontecimento where CodAcontecimento = {codAcontecimento} and IdUsuario = {idUsuario}");
                    Exec($"delete from UsuarioAcontecimento where CodAcontecimento = {codAcontecimento} and IdUsuario = {idUsuario}");
                }
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
                    t.IdUsuariosMarcados = Exec($"select IdUsuario from UsuarioTarefa where CodTarefa = {t.CodTarefa}", new List<int>());
                    t.IdUsuariosAdmin = Exec($"select IdAdmin from AdminTarefa where CodTarefa = {t.CodTarefa}", new List<int>());
                    t.Terminada = Exec($"select Terminada from UsuarioTarefa where CodTarefa = {t.CodTarefa} and IdUsuario = {id}", typeof(int)) == 1;
                }
                return lista;
            }
            public List<Tarefa> Tarefas(Meta meta)
            {
                List<Tarefa> lista = new List<Tarefa>();
                lista = Exec($"select * from Tarefa where CodTarefa in(select CodTarefa from TarefaMeta where CodMeta = {meta.CodMeta})", lista);
                foreach (Tarefa t in lista)
                {
                    t.Meta = meta;
                    t.IdUsuariosAdmin = Exec($"select IdAdmin from AdminTarefa where CodTarefa = {t.CodTarefa}", new List<int>());
                }
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
                var l = Exec($"select * from Acontecimento where CodAcontecimento in (select CodAcontecimento from UsuarioAcontecimento where IdUsuario = {id})", new List<Acontecimento>());
                foreach (Acontecimento a in l)
                {
                    a.IdUsuariosMarcados = Exec($"select IdUsuario from UsuarioAcontecimento where CodAcontecimento = {a.CodAcontecimento}", new List<int>());
                    a.IdUsuariosAdmin = Exec($"select IdUsuario from AdminAcontecimento where CodAcontecimento = {a.CodAcontecimento}", new List<int>());
                }
                return l;
            }
            public Tarefa Tarefa(int id)
            {
                Tarefa ret = Exec($"select * from Tarefa where CodTarefa = {id}", typeof(Tarefa));
                if (ret != null)
                {
                    ret.Meta = Exec($"select * from Meta where CodMeta in (select CodMeta from TarefaMeta where CodTarefa = {ret.CodTarefa})", typeof(Meta));
                    ret.IdUsuariosMarcados = Exec($"select IdUsuario from UsuarioTarefa where CodTarefa = {ret.CodTarefa}", new List<int>());
                    ret.IdUsuariosAdmin = Exec($"select IdAdmin from AdminTarefa where CodTarefa = {ret.CodTarefa}", new List<int>());
                }
                return ret;
            }
            public Acontecimento Acontecimento(int id)
            {
                Acontecimento ret = Exec($"select * from Acontecimento where CodAcontecimento = {id}", typeof(Acontecimento));
                if (ret != null)
                {
                    ret.IdUsuariosMarcados = Exec($"select IdUsuario from UsuarioAcontecimento where CodAcontecimento = {ret.CodAcontecimento}", new List<int>());
                    ret.IdUsuariosAdmin = Exec($"select IdUsuario from AdminAcontecimento where CodAcontecimento = {ret.CodAcontecimento}", new List<int>());
                }
                return ret;
            }
            public Meta Meta(int id)
            {
                return Exec($"select * from Meta where CodMeta = {id}", typeof(Meta));
            }
            public void AlterarEstadoTarefa(int t, int u, bool atual)
            {
                Exec($"update UsuarioTarefa set Terminada = {(atual?1:0)} where CodTarefa = {t} and IdUsuario = {u}");
            }
            public int[] DarRecompensa(int id, int codT)
            {
                Tarefa t = Tarefa(codT);
                Exec($"adicionarDinheiro_sp {id}, {t.Recompensa}");
                Exec($"adicionarXP_sp {id}, {t.XP}");
                int[] valores = { t.Recompensa, t.XP };
                return valores;
            }
            public int[] RetirarRecompensa(int id, int codT)
            {
                Tarefa t = Tarefa(codT);
                Exec($"adicionarDinheiro_sp {id}, {t.Recompensa * -1}");
                Exec($"adicionarXP_sp {id}, {t.XP * -1}");
                int[] valores = { t.Recompensa * -1, t.XP * -1 };
                return valores;
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
            public List<string> GetItensEquipadosDeUsuario(int id)
            {
                List<string> lista = new List<string>();
                //tarefas
                string[] conteudos = Exec($"select Titulo from Usuario where Id = {id}", typeof(string)).Split(' ');
                conteudos[0] = Exec($"select Conteudo from Item where CodItem = {conteudos[0]}", typeof(string));
                lista.Add(string.Join(" ", conteudos));
                //Insignias
                lista.Add(Exec($"select Conteudo from Item where CodItem = {Exec($"select Insignia from Usuario where Id = {id}", typeof(int))}", typeof(string)));
                //Decorações
                lista.Add(Exec($"select Conteudo from Item where CodItem = {Exec($"select Decoracao from Usuario where Id = {id}", typeof(int))}", typeof(string)));

                return lista;
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
                var x = Exec($"select * from Item where CodItem = {id}", typeof(Item));
                if (x == null)
                    throw new Exception();
                return x;
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
                List<string> tituloAtual = new List<string>(Usuarios.GetUsuario(idUsuario).Titulo.Split(' '));
                tituloAtual.Remove(e);
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
                    if (lista.GetType().GetGenericArguments()[0] != typeof(int))
                        lista.Add(lista.GetType().GetGenericArguments()[0].GetConstructor(new Type[] { typeof(SqlDataReader) }).Invoke(new object[] { ret }));
                    else
                        lista.Add(Convert.ToInt32(ret.GetValue(0)));
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
                    try
                    {
                        t = Convert.ToInt32(ret.GetValue(0));
                    } catch { t = 0; }
                    conexao.Close();
                    return t;
                }
                else if (tipo == typeof(string))
                {
                    ret.Read();
                    try
                    {
                        t = ret.GetValue(0).ToString();
                    } catch { t = ""; }
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
            catch (Exception ex) { throw ex; }
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