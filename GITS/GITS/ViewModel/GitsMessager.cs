using System;
using System.Collections.Generic;
using System.Globalization;
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

            mail.Body = conteudo;
            mail.IsBodyHtml = true;
            mail.BodyEncoding = System.Text.Encoding.UTF8;

            mail.Priority = MailPriority.Normal;

            smtp.Send(mail);
        }

        const string linkDoSite = "http://localhost:49291";
        public static void EnviarEmailDiario(Usuario usuario)
        {
            string assunto = "GITS - Relatório diário";
            string conteudo =
                "<link href=\"https://cdnjs.cloudflare.com/ajax/libs/materialize/1.0.0/css/materialize.min.css\" rel =\"stylesheet\">" +
                "<link href=\"https://fonts.googleapis.com/icon?family=Material+Icons\" rel=\"stylesheet\">" +
                $"<a href = \"{linkDoSite}\" class=\"brand-logo\" id=\"titulo\"><i class=\"material-icons\">hourglass_empty</i> GITS - Email Diário</a> " +
                "<h2>Programações para hoje:</h2>";

            bool mostrouTarefas = false;
            foreach (Tarefa t in usuario.Tarefas)
            {
                if (EhDeHoje(t.Data))
                {
                    if (!mostrouTarefas)
                        conteudo += " * Tarefas: <ul>";
                    mostrouTarefas = true;

                    conteudo += $"<li><a href=\"{linkDoSite}/tarefas/{t.CodTarefa}\">{t.Titulo}</a></li>";
                }
            }
            if (mostrouTarefas)
                conteudo += "</ul>";

            bool mostrouAcontecimentos = false;
            foreach (Acontecimento a in usuario.Acontecimentos)
            {
                if (EhDeHoje(a.Data))
                {
                    if (!mostrouAcontecimentos)
                        conteudo += " * Acontecimentos: <ul>";
                    mostrouAcontecimentos = true;

                    conteudo += $"<li><a href=\"{linkDoSite}/acontecimentos/{a.CodAcontecimento}\">{a.Titulo}</a></li>";
                }
            }
            if (mostrouAcontecimentos)
                conteudo += "</ul>";

            if (!mostrouTarefas && !mostrouAcontecimentos)
                conteudo += "<p><i>Não há nada programado para hoje.</i></p>";

            conteudo += "<br><h2>Programações para os próximos 7 dias:</h2>";

            mostrouTarefas = false;
            foreach (Tarefa t in usuario.Tarefas)
            {
                if (EhDaSemana(t.Data) && !EhDeHoje(t.Data))
                {
                    if (!mostrouTarefas)
                        conteudo += " * Tarefas: <ul>";
                    mostrouTarefas = true;

                    conteudo += $"<li><a href=\"{linkDoSite}/tarefas/{t.CodTarefa}\">{t.Titulo}</a></li>";
                }
            }
            if (mostrouTarefas)
                conteudo += "</ul>";

            mostrouAcontecimentos = false;
            foreach (Acontecimento a in usuario.Acontecimentos)
            {
                if (EhDaSemana(a.Data) && !EhDeHoje(a.Data))
                {
                    if (!mostrouAcontecimentos)
                        conteudo += " * Acontecimentos: <ul>";
                    mostrouAcontecimentos = true;

                    conteudo += $"<li><a href=\"{linkDoSite}/acontecimentos/{a.CodAcontecimento}\">{a.Titulo}</a></li>";
                }
            }
            if (mostrouAcontecimentos)
                conteudo += "</ul>";

            if (!mostrouTarefas &&  !mostrouAcontecimentos)
                conteudo += "<p><i>Não há nada programado para os próximos 7 dias.</i></p>";

            string para = usuario.Email;

            EnviarEmail(assunto, conteudo, para);
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
    }
}