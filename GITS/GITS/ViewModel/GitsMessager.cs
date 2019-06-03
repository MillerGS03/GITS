using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace GITS.ViewModel
{
    public static class GitsMessager
    {
        public static void EnviarEmail(string assunto, string conteudo, string para)
        {
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("gitsmessager@gmail.com", "MillerScherer1")
            };

            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("gitsmessager@gmail.com", "GITS");
            //mail.CC.Add(new MailAddress("gumiller2003@gmail.com"));
            //mail.CC.Add(new MailAddress("vinschers@gmail.com"));
            mail.To.Add(new MailAddress(para));

            mail.Subject = assunto;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;

            mail.Body = $"<html><head><style>{Estilo}</style></head><body><div style=\"border: 2px solid silver; border-radius: 10px; padding: 15px;\">" + ImgGits + conteudo + "</div></body></html>";
            mail.IsBodyHtml = true;
            mail.BodyEncoding = System.Text.Encoding.UTF8;

            mail.Priority = MailPriority.Normal;

            smtp.Send(mail);
        }

        const string linkDoSite = "http://localhost:49291";
        public static void EnviarEmailsDiarios()
        {
            foreach (Usuario u in Dao.ListaUsuarios)
            {
                var config = u.ConfiguracoesEmail;
                if (config.RelatorioDiario && !config.DataUltimoRelatorioEnviado.Date.Equals(DateTime.Now.Date))
                {
                    EnviarEmailDiario(u);
                    u.ConfiguracoesEmail.DataUltimoRelatorioEnviado = DateTime.Now;
                    Dao.Usuarios.AtualizarConfiguracoesEmail(u.Id, u.ConfiguracoesEmail);
                }
            }
        }
        public static void EnviarEmailDiario(Usuario usuario)
        {
            bool podeEnviar = false;

            string assunto = "GITS - Relatório diário";
            string conteudo = "<h1>Email Diário</h1>";

            bool mostrouTarefas = false;
            foreach (Tarefa t in usuario.Tarefas)
            {
                if (EhAtrasado(t.Data))
                {
                    if (!mostrouTarefas)
                        conteudo += "<h2>Tarefas atrasadas:</h2><ul style=\"list-style-type: none; padding-left: 0;\">";
                    mostrouTarefas = true;

                    conteudo += $"<a href=\"{linkDoSite}/tarefas/{t.CodTarefa}\"><li style=\"padding: 6px; border-bottom: 1px solid #dddddd;\">{t.Data} - {t.Titulo}</li></a>";
                }
            }
            if (mostrouTarefas)
            {
                conteudo += "</ul>";
                podeEnviar = true;
            }

            conteudo += "<h2>Programações para hoje:</h2>";
            mostrouTarefas = false;
            foreach (Tarefa t in usuario.Tarefas)
            {
                if (EhDeHoje(t.Data))
                {
                    if (!mostrouTarefas)
                        conteudo += "<h3>&nbsp;&nbsp;Tarefas:</h3> <ul style=\"list-style-type: none; padding-left: 0;\">";
                    mostrouTarefas = true;

                    conteudo += $"<a href=\"{linkDoSite}/tarefas/{t.CodTarefa}\"><li style=\"padding: 6px; border-bottom: 1px solid #dddddd;\">{t.Data} - {t.Titulo}</li></a>";
                }
            }
            if (mostrouTarefas)
            {
                conteudo += "</ul>";
                podeEnviar = true;
            }

            bool mostrouAcontecimentos = false;
            foreach (Acontecimento a in usuario.Acontecimentos)
            {
                if (EhDeHoje(a.Data))
                {
                    if (!mostrouAcontecimentos)
                        conteudo += "<h3> &nbsp;&nbsp;Acontecimentos:</h3> <ul style=\"list-style-type: none; padding-left: 0;\">";
                    mostrouAcontecimentos = true;

                    conteudo += $"<a href=\"{linkDoSite}/acontecimentos/{a.CodAcontecimento}\"><li style=\"padding: 6px; border-bottom: 1px solid #dddddd;\">{a.Data} - {a.Titulo}</li></a>";
                }
            }
            if (mostrouAcontecimentos)
            {
                conteudo += "</ul>";
                podeEnviar = true;
            }

            if (!mostrouTarefas && !mostrouAcontecimentos)
                conteudo += "<p><i>Não há nada programado para hoje.</i></p>";

            conteudo += "<br><h2>Programações para os próximos 7 dias:</h2>";

            mostrouTarefas = false;
            foreach (Tarefa t in usuario.Tarefas)
            {
                if (EhDaSemana(t.Data) && !EhDeHoje(t.Data))
                {
                    if (!mostrouTarefas)
                        conteudo += "<h3>&nbsp;&nbsp;Tarefas:</h3> <ul style=\"list-style-type: none; padding-left: 0;\">";
                    mostrouTarefas = true;

                    conteudo += $"<a href=\"{linkDoSite}/tarefas/{t.CodTarefa}\"><li style=\"padding: 6px; border-bottom: 1px solid #dddddd;\">{t.Data} - {t.Titulo}</li></a>";
                }
            }
            if (mostrouTarefas)
            {
                conteudo += "</ul>";
                podeEnviar = true;
            }

            mostrouAcontecimentos = false;
            foreach (Acontecimento a in usuario.Acontecimentos)
            {
                if (EhDaSemana(a.Data) && !EhDeHoje(a.Data))
                {
                    if (!mostrouAcontecimentos)
                        conteudo += "<h3> &nbsp;&nbsp;Acontecimentos:</h3> <ul style=\"list-style-type: none; padding-left: 0;\">";
                    mostrouAcontecimentos = true;

                    conteudo += $"<a href=\"{linkDoSite}/acontecimentos/{a.CodAcontecimento}\"><li style=\"padding: 6px; border-bottom: 1px solid #dddddd;\">{a.Data} - {a.Titulo}</li></a>";
                }
            }
            if (mostrouAcontecimentos)
            {
                conteudo += "</ul>";
                podeEnviar = true;
            }

            if (!mostrouTarefas && !mostrouAcontecimentos)
                conteudo += "<p><i>Não há nada programado para os próximos 7 dias.</i></p>";

            string para = usuario.Email;

            if (podeEnviar)
                EnviarEmail(assunto, conteudo, para);
        }
        private static bool EhAtrasado(string data)
        {
            var hoje = DateTime.Now;
            var dataD = DateTime.Parse(data, CultureInfo.GetCultureInfo("pt-BR"));
            return dataD.Ticks < hoje.Ticks && (dataD.Day != hoje.Day || dataD.Month != hoje.Month || dataD.Year != hoje.Year);
        }
        private static bool EhDeHoje(string data)
        {
            var hoje = DateTime.Now;
            var dataD = DateTime.Parse(data, CultureInfo.GetCultureInfo("pt-BR"));
            return hoje.Day == dataD.Day && hoje.Month == dataD.Month && hoje.Year == dataD.Year;
        }
        private static bool EhDaSemana(string data)
        {
            var hoje = DateTime.Now;
            var dataD = DateTime.Parse(data, CultureInfo.GetCultureInfo("pt-BR"));

            if (dataD.Ticks < hoje.Ticks)
                return false;

            var intervalo = new TimeSpan(dataD.Ticks - hoje.Ticks);

            return intervalo.TotalDays <= 7;
        }
        private static string estilo;
        public static string Estilo
        {
            get
            {
                try
                {
                    if (estilo == null || estilo == "")
                    {
                        estilo = "";

                        StreamReader leitor = new StreamReader(HttpContext.Current.Server.MapPath("~/Content/estiloEmail.css"));
                        while (!leitor.EndOfStream)
                            estilo += leitor.ReadLine();
                        leitor.Close();
                    }

                    return estilo;
                }
                catch { return ""; }
            }
        }
        public static string ImgGits { get => "<img style=\"width: 160px; max-width: 100%;\" src=\"https://res.cloudinary.com/dlxa1xvrc/image/upload/v1559595012/gits_atv5az.png\">"; }
    }
}