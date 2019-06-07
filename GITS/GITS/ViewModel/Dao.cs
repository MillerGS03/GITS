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
                List<int> desenvolvedores = Exec("select IdUsuario from Desenvolvedor", new List<int>());
                if (usuarios != null)
                {
                    foreach (Usuario u in usuarios)
                    {
                        u.Amigos = Amigos(u.Id, true);
                        u.Solicitacoes = Amigos(u.Id, false);
                        u.Tarefas = Eventos.Tarefas(u.Id, true);
                        u.Metas = Eventos.Metas(u.Id);
                        u.Acontecimentos = Eventos.Acontecimentos(u.Id);
                        u.Notificacoes = Notificacoes(u.Id);
                        u.Itens = Itens.GetItensDeUsuario(u.Id);
                        u.ConfiguracoesEmail = GetEmailConfig(u.Id);
                        u.Desenvolvedor = desenvolvedores.Contains(u.Id);
                    }
                }
                ListaUsuarios = usuarios;
                return usuarios;
            }
            public Usuario GetUsuario(int id)
            {
                try
                {
                    return ListaUsuarios.Find(u => u.Id == id);
                }
                catch
                {
                    return GetUsuarioDoBanco(id);
                }
            }
            public Usuario GetUsuarioDoBanco(int id)
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
                        a.ConfiguracoesEmail = GetEmailConfig(id);
                        return a;
                    }
                    return null;
                }
                catch { return null; }
            }
            public EmailConfig GetEmailConfig(int idUsuario)
            {
                return Exec($"select * from ConfiguracaoEmail where IdUsuario = {idUsuario}", typeof(EmailConfig));
            }
            public void AtualizarConfiguracoesEmail(int idUsuario, EmailConfig configuracoes)
            {
                Exec($"update ConfiguracaoEmail set RelatorioDiario={(configuracoes.RelatorioDiario ? 1 : 0)}, " +
                                                  $"RequisicoesAdministracao={(configuracoes.RequisicoesAdministracao ? 1 : 0)}, " +
                                                  $"PedidosAmizade={(configuracoes.PedidosAmizade ? 1 : 0)}, " +
                                                  $"NotificacoesAmizadesAceitas={(configuracoes.NotificacoesAmizadesAceitas ? 1 : 0)}, " +
                                                  $"RequisicoesEntrar={(configuracoes.RequisicoesEntrar ? 1 : 0)}, " +
                                                  $"AvisosSaida={(configuracoes.AvisosSaida ? 1 : 0)}, " +
                                                  $"TornouSeAdm={(configuracoes.TornouSeAdm ? 1 : 0)}, " +
                                                  $"ConviteTarefaAcontecimento={(configuracoes.ConviteTarefaAcontecimento ? 1 : 0)}, " +
                                                  $"MarcadoEmPost={(configuracoes.MarcadoEmPost ? 1 : 0)}");
                var user = ListaUsuarios.Find(u => u.Id == idUsuario);
                if (!configuracoes.DataUltimoRelatorioEnviado.Equals(new DateTime()))
                    Exec($"update ConfiguracaoEmail set DataUltimoRelatorioEnviado='{configuracoes.DataUltimoRelatorioEnviado.ToString()}' where IdUsuario={idUsuario}");
                else
                    configuracoes.DataUltimoRelatorioEnviado = user.ConfiguracoesEmail.DataUltimoRelatorioEnviado;
                user.ConfiguracoesEmail = configuracoes;
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
                        a = ListaUsuarios.Find(u => u.Id == id);
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
                var retornoId = GetUsuarioDoBanco(u.Id);

                if (retornoId.Id > 0)
                {
                    ListaUsuarios.Add(retornoId);
                    return retornoId.Id;
                }
                else
                    throw new Exception("Erro na inserção!");
            }
            public void Remove(Usuario u)
            {
                Usuario s = Exec($"select * from Usuario where Id = {u.Id}", typeof(Usuario));
                if (s != null)
                {
                    Exec($"removerUsuario {u.Id}");
                    ListaUsuarios.RemoveAll(user => user.Id == u.Id);
                    foreach (Usuario user in ListaUsuarios)
                    {
                        var i = user.Amigos.FindIndex(a => a.Id == u.Id);
                        if (i >= 0)
                            user.Amigos = Amigos(user.Id, true);
                    }
                }
            }
            public void Remove(int id)
            {
                Usuario s = Exec($"select * from Usuario where Id = {id}", typeof(Usuario));
                if (s != null)
                {
                    Exec($"removerUsuario {id}");
                    ListaUsuarios.RemoveAll(user => user.Id == id);
                    foreach (Usuario user in ListaUsuarios)
                    {
                        var i = user.Amigos.FindIndex(a => a.Id == id);
                        if (i >= 0)
                            user.Amigos = Amigos(user.Id, true);
                    }
                }
            }
            public void Update(Usuario u)
            {
                Usuario s = Exec($"select * from Usuario where Id = {u.Id}", typeof(Usuario));
                if (s != null)
                {
                    Exec($"update Usuario set Nome = '{u.Nome}', FotoPerfil = '{u.FotoPerfil}', Email = '{u.Email}', Decoracao = {u.Decoracao}, TemaSite = {u.TemaSite}, Dinheiro = {u.Dinheiro}, Titulo = '{u.Titulo}', _Status = '{u.Status}', XP = {u.XP} where Id = {u.Id}");
                    AtualizarConfiguracoesEmail(u.Id, u.ConfiguracoesEmail);
                }
                ListaUsuarios.RemoveAll(user => user.Id == u.Id);
                ListaUsuarios.Add(u);

                foreach (Usuario user in ListaUsuarios)
                {
                    var i = user.Amigos.FindIndex(a => a.Id == u.Id);
                    if (i >= 0)
                        user.Amigos = Amigos(user.Id, true);
                }
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

                    Usuario user = null;
                    try
                    {
                        user = ListaUsuarios.Find(u => u.Id == idAtual);
                    }
                    catch
                    {
                        user = Exec($"select * from Usuario where Id = {idAtual}", typeof(Usuario));
                    }

                    ret.Add(new Amigo(user, aceito, a1 == id));
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
                CriarNotificacao(new Notificacao(dois, um, 2, Exec($"select CodAmizade from Amizade where (CodUsuario1 = {um} or CodUsuario2 = {um}) and (CodUsuario1 = {dois} or CodUsuario2 = {dois})", typeof(int)), false));

                var usuarioUm = ListaUsuarios.Find(u => u.Id == um);
                var usuarioDois = ListaUsuarios.Find(u => u.Id == dois);
                usuarioUm.Amigos.Add(new Amigo(usuarioDois, false, true));
                usuarioDois.Amigos.Add(new Amigo(usuarioUm, false, false));
            }
            public void RemoverAmizade(int um, int dois)
            {
                Amizade s = Exec($"select * from Amizade where (CodUsuario1 = {um} or CodUsuario2 = {um}) and (CodUsuario1 = {dois} or CodUsuario2 = {dois})", typeof(Amizade));
                if (s.CodUsuario1 == 0)
                    throw new Exception("Amizade nao existe");
                Exec($"delete from Amizade where (CodUsuario1 = {um} or CodUsuario2 = {um}) and (CodUsuario1 = {dois} or CodUsuario2 = {dois})");

                ListaUsuarios.Find(u => u.Id == um).Amigos.RemoveAll(a => a.Id == dois);
                ListaUsuarios.Find(u => u.Id == dois).Amigos.RemoveAll(a => a.Id == um);
            }
            public void AceitarAmizade(int codAmizade, Notificacao n)
            {
                Amizade s = Exec($"select * from Amizade where CodAmizade = {codAmizade}", typeof(Amizade));
                if (s == null)
                    throw new Exception("Amizade não existe");
                Exec($"update Amizade set FoiAceito = 1 where CodAmizade = {codAmizade}");

                ListaUsuarios.Find(u => u.Id == s.CodUsuario1).Amigos = Amigos(s.CodUsuario1, true);
                ListaUsuarios.Find(u => u.Id == s.CodUsuario2).Amigos = Amigos(s.CodUsuario2, true);

                CriarNotificacao(n);
            }
            public int AceitarAmizade(int idAceitou, int idMandou)
            {
                Amizade s = Exec($"select * from Amizade where FoiAceito = 0 and CodUsuario1 = {idMandou} and CodUsuario2 = {idAceitou}", typeof(Amizade));
                if (s == null)
                    throw new Exception("Amizade não existe");
                Exec($"update Amizade set FoiAceito = 1 where CodAmizade = {s.CodAmizade}");

                ListaUsuarios.Find(u => u.Id == s.CodUsuario1).Amigos = Amigos(s.CodUsuario1, true);
                ListaUsuarios.Find(u => u.Id == s.CodUsuario2).Amigos = Amigos(s.CodUsuario2, true);

                CriarNotificacao(new Notificacao(idMandou, idAceitou, 3, s.CodAmizade, false));
                return s.CodAmizade;
            }
            public void RecusarAmizade(int codAmizade)
            {
                Amizade s = Exec($"select * from Amizade where CodAmizade = {codAmizade}", typeof(Amizade));
                if (s == null)
                    throw new Exception("Amizade não existe");

                Exec($"delete from Amizade where CodAmizade = {codAmizade}");

                ListaUsuarios.Find(u => u.Id == s.CodUsuario1).Amigos = Amigos(s.CodUsuario1, true);
                ListaUsuarios.Find(u => u.Id == s.CodUsuario2).Amigos = Amigos(s.CodUsuario2, true);
            }
            public int RecusarAmizade(int idRecusou, int idMandou)
            {
                Amizade s = Exec($"select * from Amizade where FoiAceito = 0 and CodUsuario1 = {idMandou} and CodUsuario2 = {idRecusou}", typeof(Amizade));
                if (s == null)
                    throw new Exception("Amizade não existe");

                Exec($"delete from Amizade where CodAmizade = {s.CodAmizade}");

                ListaUsuarios.Find(u => u.Id == s.CodUsuario1).Amigos = Amigos(s.CodUsuario1, true);
                ListaUsuarios.Find(u => u.Id == s.CodUsuario2).Amigos = Amigos(s.CodUsuario2, true);

                return s.CodAmizade;
            }
            public void CriarNotificacao(Notificacao n)
            {
                Notificacao s = Exec($"select * from Notificacao where Id = {n.Id}", typeof(Notificacao));
                if (s.Id != 0)
                    throw new Exception("Notificacao ja existe");
                Exec($"insert into Notificacao values({n.IdUsuarioReceptor}, {n.IdUsuarioTransmissor}, {n.Tipo}, {n.IdCoisa}, {(n.JaViu ? 1 : 0)})");
                var user = ListaUsuarios.Find(u => u.Id == n.IdUsuarioReceptor);
                user.Notificacoes.Add(Exec($"select * from Notificacao where Id = {n.Id}", typeof(Notificacao)));
                GitsMessager.Notificar(n, user);
            }

            public void RemoverNotificacao(int n)
            {
                Notificacao s = Exec($"select * from Notificacao where Id = {n}", typeof(Notificacao));
                if (s.Id != 0)
                {
                    Exec($"delete from Notificacao where Id = {n}");
                    ListaUsuarios.Find(u => u.Id == s.IdUsuarioReceptor).Notificacoes.RemoveAll(not => not.Id == s.Id);
                }
                else
                    throw new Exception("Notificacao nao existe");
            }
            public void VisualizarNotificacao(int c)
            {
                Notificacao s = Exec($"select * from Notificacao where Id = {c}", typeof(Notificacao));
                if (s.Id != 0)
                {
                    Exec($"update Notificacao set JaViu = 1 where Id = {c}");
                    ListaUsuarios.Find(u => u.Id == s.IdUsuarioReceptor).Notificacoes.Find(n => n.Id == c).JaViu = true;
                }
                else
                    throw new Exception("Notificacao nao existe");
            }
            public void VisualizarNotificacao(int tipo, int transmissor)
            {
                List<Notificacao> s = Exec($"select * from Notificacao where Tipo = {tipo} and IdUsuarioTransmissor = {transmissor}", new List<Notificacao>());
                if (s.Count != 0)
                {
                    Exec($"update Notificacao set JaViu = 1 where Tipo = {tipo} and IdUsuarioTransmissor = {transmissor}");
                    foreach (Notificacao n in s)
                    {
                        ListaUsuarios.Find(u => u.Id == n.IdUsuarioReceptor).Notificacoes.Find(notif => notif.Id == n.Id).JaViu = true;
                    }
                }
                else
                    throw new Exception("Notificacao nao existe");
            }
            public void Publicar(Publicacao publicacao, int[] idsUsuariosMarcados)
            {
                var x = publicacao.Data.ToString();
                publicacao.IdPublicacao = Exec($"publicar_sp {publicacao.IdUsuario}, '{publicacao.Titulo}', '{publicacao.Descricao}', '{publicacao.Data.ToString()}', {publicacao.Likes}, {(publicacao.ComentarioDe == 0 ? "null" : publicacao.ComentarioDe.ToString())}", typeof(int));
                if (idsUsuariosMarcados != null && idsUsuariosMarcados.Length > 0)
                    foreach (int id in idsUsuariosMarcados)
                    {
                        Usuarios.CriarNotificacao(new Notificacao(publicacao, id));
                        Exec($"insert into UsuarioMarcadoPublicacao values ({id}, {publicacao.IdPublicacao})");
                    }
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
                Exec($"insert into Meta values ('{meta.Titulo}', '{meta.Descricao}', {(meta.Data == null ? "null" : $"'{meta.Data}'")}, {meta.Progresso}, '{meta.UltimaInteracao}', {meta.Recompensa.ToString().Replace(",", ".")}, {meta.GitcoinsObtidos}, {meta.TarefasCompletas})");
                var idMeta = Exec($"select max(CodMeta) from Meta", typeof(int));
                Exec($"insert into UsuarioMeta values ({idCriador}, {idMeta})");

                meta.CodMeta = idMeta;
                ListaUsuarios.Find(u => u.Id == idCriador).Metas.Add(meta);

            }
            public void AtualizarMeta(Meta meta, int idCriador)
            {
                if (Exec($"select count(*) from Meta where CodMeta={meta.CodMeta} and CodMeta in (select CodMeta from UsuarioMeta where IdUsuario={idCriador})", typeof(int)) == 0)
                    throw new Exception("A meta não existe!");

                Exec($"update Meta set Titulo='{meta.Titulo}', Descricao='{meta.Descricao}', Data={(meta.Data == null ? "null" : $"'{meta.Data}'")}, Recompensa={meta.Recompensa} where CodMeta={meta.CodMeta}");
                var metas = ListaUsuarios.Find(u => u.Id == idCriador).Metas;
                metas.RemoveAll(m => m.CodMeta == meta.CodMeta);
                metas.Add(meta);
            }
            public void RemoverMeta(int idCriador, int idMeta)
            {
                if (Exec($"select count(*) from Meta where CodMeta={idMeta} and CodMeta in (select CodMeta from UsuarioMeta where IdUsuario={idCriador})", typeof(int)) == 0)
                    throw new Exception("A meta não existe!");


                Exec("RemoverMeta_sp " + idMeta);
                ListaUsuarios.Find(u => u.Id == idCriador).Metas.RemoveAll(m => m.CodMeta == idMeta);
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
                t.CodTarefa = Exec($"adicionarTarefa '{t.Data}', '{t.Titulo}', '{t.Descricao}', {t.Dificuldade}, {t.IdUsuariosAdmin[0]}, {(t.Meta == null ? 0 : t.Meta.CodMeta)}, {t.Recompensa}, '{t.Criacao}', {t.XP}", typeof(int));
                int idCriador = t.IdUsuariosAdmin[0];
                ListaUsuarios.Find(u => u.Id == idCriador).Tarefas.Add(t);
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

                    foreach (int idUsuario in s.IdUsuariosAdmin)
                        ListaUsuarios.Find(u => u.Id == idUsuario).Tarefas.RemoveAll(tarefa => tarefa.CodTarefa == t);
                    foreach (int idUsuario in s.IdUsuariosMarcados)
                        ListaUsuarios.Find(u => u.Id == idUsuario).Tarefas.RemoveAll(tarefa => tarefa.CodTarefa == t);
                }
                else
                    throw new Exception("Tarefa nao existe");
            }
            public void UpdateTarefaFull(Tarefa t, int idUser, int idMetaAnterior = 0)
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

                var tarefaAtualizada = Tarefa(t.CodTarefa, idUser);
                var novaMeta = tarefaAtualizada.Meta;

                foreach (int idUsuario in t.IdUsuariosAdmin)
                {
                    var tarefas = ListaUsuarios.Find(u => u.Id == idUsuario).Tarefas;
                    if (idUsuario != idUser)
                        tarefaAtualizada.Meta = tarefas.Find(tarefa => tarefa.CodTarefa == t.CodTarefa).Meta;
                    else
                        tarefaAtualizada.Meta = novaMeta;

                    tarefas.RemoveAll(tarefa => tarefa.CodTarefa == t.CodTarefa);
                    tarefas.Add(tarefaAtualizada);
                }
                foreach (int idUsuario in t.IdUsuariosMarcados)
                {
                    if (!t.IdUsuariosAdmin.Contains(idUsuario))
                    {
                        var tarefas = ListaUsuarios.Find(u => u.Id == idUsuario).Tarefas;
                        if (idUsuario != idUser)
                            tarefaAtualizada.Meta = tarefas.Find(tarefa => tarefa.CodTarefa == t.CodTarefa).Meta;
                        else
                            tarefaAtualizada.Meta = novaMeta;

                        tarefas.RemoveAll(tarefa => tarefa.CodTarefa == t.CodTarefa);
                        tarefas.Add(tarefaAtualizada);
                    }
                }
            }
            public void UpdateTarefa(Tarefa t, int idUser, int idMetaAnterior = 0)
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

                var tarefaAtualizada = Tarefa(t.CodTarefa, idUser);
                var novaMeta = tarefaAtualizada.Meta;

                foreach (int idUsuario in t.IdUsuariosAdmin)
                {
                    var tarefas = ListaUsuarios.Find(u => u.Id == idUsuario).Tarefas;
                    if (idUsuario != idUser)
                        tarefaAtualizada.Meta = tarefas.Find(tarefa => tarefa.CodTarefa == t.CodTarefa).Meta;
                    else
                        tarefaAtualizada.Meta = novaMeta;

                    tarefas.RemoveAll(tarefa => tarefa.CodTarefa == t.CodTarefa);
                    tarefas.Add(tarefaAtualizada);
                }
                foreach (int idUsuario in t.IdUsuariosMarcados)
                {
                    if (!t.IdUsuariosAdmin.Contains(idUsuario))
                    {
                        var tarefas = ListaUsuarios.Find(u => u.Id == idUsuario).Tarefas;
                        if (idUsuario != idUser)
                            tarefaAtualizada.Meta = tarefas.Find(tarefa => tarefa.CodTarefa == t.CodTarefa).Meta;
                        else
                            tarefaAtualizada.Meta = novaMeta;

                        tarefas.RemoveAll(tarefa => tarefa.CodTarefa == t.CodTarefa);
                        tarefas.Add(tarefaAtualizada);
                    }
                }
            }
            public void UpdateAcontecimento(Acontecimento a)
            {
                Exec($"update Acontecimento set Titulo = '{a.Titulo}', Descricao = '{a.Descricao}', Tipo = {a.Tipo}, Data = '{a.Data}'");
                Acontecimento acontecimentoAtualizado = Acontecimento(a.CodAcontecimento);

                foreach (int idUsuario in a.IdUsuariosAdmin)
                {
                    var acontecimentos = ListaUsuarios.Find(u => u.Id == idUsuario).Acontecimentos;
                    acontecimentos.RemoveAll(acontecimento => acontecimento.CodAcontecimento == a.CodAcontecimento);
                    acontecimentos.Add(acontecimentoAtualizado);
                }
                foreach (int idUsuario in a.IdUsuariosMarcados)
                {
                    var acontecimentos = ListaUsuarios.Find(u => u.Id == idUsuario).Acontecimentos;
                    acontecimentos.RemoveAll(acontecimento => acontecimento.CodAcontecimento == a.CodAcontecimento);
                    acontecimentos.Add(acontecimentoAtualizado);

                }
            }
            public void RequisitarAdminTarefa(int t, int u)
            {
                Usuarios.CriarNotificacao(new Notificacao(Tarefa(t, u).IdUsuariosAdmin[0], u, 4, t, false));
            }
            public void AdicionarAdminATarefa(int t, int u)
            {
                Exec($"insert into AdminTarefa values({u}, {t})");

                foreach (Usuario user in ListaUsuarios)
                    try
                    {
                        user.Tarefas.Find(tarefa => tarefa.CodTarefa == t).IdUsuariosAdmin.Add(u);
                    }
                    catch { }
                Usuarios.CriarNotificacao(new Notificacao(u, 0, 11, t, false));
            }
            public void RequisitarAdminAcontecimento(int a, int u)
            {
                Usuarios.CriarNotificacao(new Notificacao(Acontecimento(a).IdUsuariosAdmin[0], u, 4, a, false));
            }
            public void AdicionarAdminAAcontecimento(int a, int u)
            {
                Exec($"insert into AdminAcontecimento values({u}, {a})");

                foreach (Usuario user in ListaUsuarios)
                    try
                    {
                        user.Acontecimentos.Find(acontecimento => acontecimento.CodAcontecimento == a).IdUsuariosAdmin.Add(u);
                    }
                    catch { }
                Usuarios.CriarNotificacao(new Notificacao(u, 0, 12, a, false));
            }
            public void CriarAcontecimento(ref Acontecimento a)
            {
                Acontecimento s = Exec($"select * from Acontecimento where CodAcontecimento = {a.CodAcontecimento}", typeof(Acontecimento));
                if (s.CodAcontecimento != 0)
                    throw new Exception("Acontecimento ja existe");
                a.CodAcontecimento = Exec($"adicionarAcontecimento_sp '{a.Titulo}', '{a.Descricao}', '{a.Data}', {a.Tipo}, {a.IdUsuariosAdmin[0]}", typeof(int));

                int idCriador = a.IdUsuariosAdmin[0];
                ListaUsuarios.Find(u => u.Id == idCriador).Acontecimentos.Add(a);
            }
            public void RemoverAcontecimento(int a)
            {
                Acontecimento s = Exec($"select * from Acontecimento where CodAcontecimento = {a}", typeof(Acontecimento));
                if (s.CodAcontecimento != 0)
                {
                    Exec($"delete from AdminAcontecimento where CodAcontecimento = {a}");
                    Exec($"delete from UsuarioAcontecimento where CodAcontecimento = {a}");
                    Exec($"delete from Acontecimento where CodAcontecimento = {a}");

                    foreach (int idUsuario in s.IdUsuariosAdmin)
                        ListaUsuarios.Find(u => u.Id == idUsuario).Acontecimentos.RemoveAll(acontecimento => acontecimento.CodAcontecimento == a);
                    foreach (int idUsuario in s.IdUsuariosMarcados)
                        ListaUsuarios.Find(u => u.Id == idUsuario).Acontecimentos.RemoveAll(acontecimento => acontecimento.CodAcontecimento == a);
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

                foreach (Usuario user in ListaUsuarios)
                {
                    try
                    {
                        user.Tarefas.Find(t => t.CodTarefa == codTarefa).IdUsuariosMarcados.Add(idUsuario);
                    }
                    catch { if (user.Id == idUsuario) user.Tarefas = Tarefas(user.Id, true); }
                }
            }
            public void RemoverUsuarioDeTarefa(int idUsuario, int codTarefa)
            {
                Tarefa s = Exec($"select * from UsuarioTarefa where CodTarefa = {codTarefa} and IdUsuario = {idUsuario}", typeof(Tarefa));
                if (s.CodTarefa != 0)
                {
                    if (s.IdUsuariosAdmin.Contains(idUsuario))
                        Exec($"delete from AdminTarefa where CodTarefa = {codTarefa} and IdAdmin = {idUsuario}");
                    Exec($"delete from UsuarioTarefa where CodTarefa = {codTarefa} and IdUsuario = {idUsuario}");

                    foreach (Usuario user in ListaUsuarios)
                    {
                        try
                        {
                            var tarefa = user.Tarefas.Find(t => t.CodTarefa == codTarefa);
                            tarefa.IdUsuariosAdmin.Remove(idUsuario);
                            tarefa.IdUsuariosMarcados.Remove(idUsuario);
                            if (tarefa.IdUsuariosAdmin.Contains(user.Id))
                                Usuarios.CriarNotificacao(new Notificacao(user.Id, idUsuario, 9, codTarefa, false));
                        }
                        catch { }
                    }
                    ListaUsuarios.Find(u => u.Id == idUsuario).Tarefas.RemoveAll(t => t.CodTarefa == codTarefa);
                }
                else
                    throw new Exception("Tarefa ou usuario invalido");
            }
            public void AdicionarUsuarioAAcontecimento(int idUsuario, int codAcontecimento)
            {
                Acontecimento s = Exec($"select * from Acontecimento where CodAcontecimento = {codAcontecimento}", typeof(Acontecimento));
                if (s.CodAcontecimento != 0)
                {
                    Exec($"insert into UsuarioAcontecimento values({idUsuario}, {codAcontecimento})");
                    Usuarios.CriarNotificacao(new Notificacao(idUsuario, s.IdUsuariosAdmin[0], 1, s.CodAcontecimento, false));
                    foreach (Usuario user in ListaUsuarios)
                    {
                        try
                        {
                            user.Acontecimentos.Find(a => a.CodAcontecimento == codAcontecimento).IdUsuariosMarcados.Add(idUsuario);
                        }
                        catch { if (user.Id == idUsuario) user.Acontecimentos = Acontecimentos(user.Id); }
                    }
                }
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

                    foreach (Usuario user in ListaUsuarios)
                    {
                        try
                        {
                            var acontecimento = user.Acontecimentos.Find(a => a.CodAcontecimento == codAcontecimento);
                            acontecimento.IdUsuariosAdmin.Remove(idUsuario);
                            acontecimento.IdUsuariosMarcados.Remove(idUsuario);
                            if (acontecimento.IdUsuariosAdmin.Contains(user.Id))
                                Usuarios.CriarNotificacao(new Notificacao(user.Id, idUsuario, 10, codAcontecimento, false));
                        }
                        catch { }
                    }
                    ListaUsuarios.Find(u => u.Id == idUsuario).Acontecimentos.RemoveAll(a => a.CodAcontecimento == codAcontecimento);
                }
                else
                    throw new Exception("Usuario ou acontecimento invalido");
            }
            public List<Tarefa> Tarefas(int id, bool aceita)
            {
                List<Tarefa> lista = new List<Tarefa>();
                lista = Exec($"select * from Tarefa where CodTarefa in(select CodTarefa from UsuarioTarefa where IdUsuario = {id} and FoiAceita = {(aceita ? 1 : 0)}) order by Data", lista);
                foreach (Tarefa t in lista)
                {
                    t.Meta = Exec($"select * from Meta where CodMeta in (select CodMeta from TarefaMeta where CodTarefa = {t.CodTarefa}) and CodMeta in (select CodMeta from UsuarioMeta where IdUsuario={id})", typeof(Meta));
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
                var l = Exec($"select * from Acontecimento where CodAcontecimento in (select CodAcontecimento from UsuarioAcontecimento where IdUsuario = {id}) order by Data", new List<Acontecimento>());
                foreach (Acontecimento a in l)
                {
                    a.IdUsuariosMarcados = Exec($"select IdUsuario from UsuarioAcontecimento where CodAcontecimento = {a.CodAcontecimento}", new List<int>());
                    a.IdUsuariosAdmin = Exec($"select IdUsuario from AdminAcontecimento where CodAcontecimento = {a.CodAcontecimento}", new List<int>());
                }
                return l;
            }
            public List<Acontecimento> Acontecimentos()
            {
                var l = Exec($"select * from Acontecimento order by Data", new List<Acontecimento>());
                foreach (Acontecimento a in l)
                {
                    a.IdUsuariosMarcados = Exec($"select IdUsuario from UsuarioAcontecimento where CodAcontecimento = {a.CodAcontecimento}", new List<int>());
                    a.IdUsuariosAdmin = Exec($"select IdUsuario from AdminAcontecimento where CodAcontecimento = {a.CodAcontecimento}", new List<int>());
                }
                return l;
            }
            public Tarefa Tarefa(int id, int idUsuario)
            {
                Tarefa ret = Exec($"select * from Tarefa where CodTarefa = {id}", typeof(Tarefa));
                if (ret != null)
                {
                    ret.Meta = Exec($"select * from Meta where CodMeta in (select CodMeta from TarefaMeta where CodTarefa = {ret.CodTarefa}) and CodMeta in (select CodMeta from UsuarioMeta where IdUsuario={idUsuario})", typeof(Meta));
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
                Exec($"update UsuarioTarefa set Terminada = {(atual ? 1 : 0)} where CodTarefa = {t} and IdUsuario = {u}");

                foreach (Usuario user in ListaUsuarios)
                    try
                    {
                        user.Tarefas.Find(tarefa => tarefa.CodTarefa == t).Terminada = atual;
                    }
                    catch { }
            }
            public int[] DarRecompensa(int id, int codT)
            {
                var user = ListaUsuarios.Find(u => u.Id == id);

                Tarefa t = user.Tarefas.Find(tarefa => tarefa.CodTarefa == codT);
                user.Dinheiro += t.Recompensa;
                user.XP += t.XP;

                Exec($"adicionarDinheiro_sp {id}, {t.Recompensa}");
                Exec($"adicionarXP_sp {id}, {t.XP}");
                int[] valores = { t.Recompensa, t.XP };
                return valores;
            }
            public int[] RetirarRecompensa(int id, int codT)
            {
                var user = ListaUsuarios.Find(u => u.Id == id);

                Tarefa t = user.Tarefas.Find(tarefa => tarefa.CodTarefa == codT);
                user.Dinheiro -= t.Recompensa;
                user.XP -= t.XP;

                Exec($"adicionarDinheiro_sp {id}, {t.Recompensa * -1}");
                Exec($"adicionarXP_sp {id}, {t.XP * -1}");
                int[] valores = { t.Recompensa * -1, t.XP * -1 };
                return valores;
            }
        }
        public static class ForumDao
        {
            public static List<Publicacao> Publicacoes(string pesquisa)
            {
                if (pesquisa != null && pesquisa != "")
                {
                    pesquisa = pesquisa.ToLower();
                    return Dao.Exec($"select * from Publicacao where ComentarioDe is null and (lower(Titulo) like '%{pesquisa}%' or lower(Descricao) like '%{pesquisa}%' or CodUsuario in(select Id from Usuario where lower(Nome) like '%{pesquisa}%'))", new List<Publicacao>());
                }
                return Dao.Exec("select * from Publicacao where ComentarioDe is null", new List<Publicacao>());
            }
            public static List<Usuario> Usuarios(string pesquisa)
            {
                if (pesquisa != null && pesquisa != "")
                    return ListaUsuarios.FindAll(u => u.Nome.ToLower().Contains(pesquisa.ToLower()));
                return ListaUsuarios;
            }
            public static List<Acontecimento> Eventos(string pesquisa)
            {
                if (pesquisa != null && pesquisa != "")
                {
                    pesquisa = pesquisa.ToLower();
                    return Dao.Exec($"select * from Acontecimento where (lower(Titulo) like '%{pesquisa}%' or CodUsuarioCriador in(select Id from Usuario where lower(Nome) like '%{pesquisa}%'))", new List<Acontecimento>());
                }
                return Dao.Exec($"select * from Acontecimento", new List<Acontecimento>());
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
                var user = ListaUsuarios.Find(u => u.Id == idUsuario);
                var item = GetItem(idItem);
                user.Itens.Add(item);
                user.Dinheiro -= item.Valor;
            }
            public void TrocarTitulo(int t, int idUsuario)
            {
                string tituloAtual = Usuarios.GetUsuario(idUsuario).Titulo;
                string[] partesTitulo = tituloAtual.Split(' ');
                partesTitulo[0] = t.ToString();
                string tituloNovo = string.Join(" ", partesTitulo);
                Exec($"update Usuario set Titulo = '{tituloNovo}' where Id = {idUsuario}");
                ListaUsuarios.Find(u => u.Id == idUsuario).Titulo = GetItem(t).Conteudo;
            }
            public void TrocarTitulo(string e, int idUsuario)
            {
                string tituloAtual = Usuarios.GetUsuario(idUsuario).Titulo;
                Exec($"update Usuario set Titulo = '{tituloAtual + " " + e}' where Id = {idUsuario}");
                ListaUsuarios.Find(u => u.Id == idUsuario).Titulo = e;
            }
            public void TirarEfeitoDeTitulo(string e, int idUsuario)
            {
                var user = ListaUsuarios.Find(u => u.Id == idUsuario);
                List<string> tituloAtual = new List<string>(user.Titulo.Split(' '));
                tituloAtual.Remove(e);
                var tituloUnido = string.Join(" ", tituloAtual);
                Exec($"update Usuario set Titulo = '{tituloUnido}' where Id = {idUsuario}");
                user.Titulo = tituloUnido;
            }
        }

        private const string conexaoBD = "Data Source = regulus.cotuca.unicamp.br; Initial Catalog =PR118179;User ID =PR118179;Password=MillerScherer1;Min Pool Size=5;Max Pool Size=250;MultipleActiveResultSets=true;";
        private static SqlConnection conexao = new SqlConnection(conexaoBD);
        private static SqlCommand comando;
        public static List<Usuario> ListaUsuarios = Usuarios.ToList();

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
            catch (Exception ex) { return null; }
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
                    }
                    catch { t = 0; }
                    conexao.Close();
                    return t;
                }
                else if (tipo == typeof(string))
                {
                    ret.Read();
                    try
                    {
                        t = ret.GetValue(0).ToString();
                    }
                    catch { t = ""; }
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