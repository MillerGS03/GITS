using System;
using System.Collections.Generic;
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
            mail.CC.Add(new MailAddress("gumiller2003@gmail.com"));
            mail.CC.Add(new MailAddress("vinschers@gmail.com"));
            mail.To.Add(new MailAddress(para));

            mail.Subject = assunto;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;

            mail.Body = conteudo;
            mail.IsBodyHtml = true;
            mail.BodyEncoding = System.Text.Encoding.UTF8;

            mail.Priority = MailPriority.Normal;

            smtp.Send(mail);
        }
    }
}