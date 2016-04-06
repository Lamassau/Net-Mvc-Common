using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Web;
using Postal;
using System.Net;
using System.Web.Mvc;
using TeknoMobi.Common.Server;

namespace TeknoMobi.Common.Helpers
{
    public static class MailHelper
    {
        public static MailMessage CreatePostalMessage(String viewName, List<KeyValuePair<string, object>> EmailValues)
        {
            IEmailService emailService = new EmailService(ViewEngines.Engines, (() => GetSmtpClient()));
 

            Email  email = new Email(viewName);
            foreach (KeyValuePair<string, object> arg in EmailValues)
            {
                email.ViewData.Add(arg.Key, arg.Value); 
            }

            return emailService.CreateMailMessage(email); ;
        }

        public  static MailMessage CreateMessage(String From, string To, string CC, string BCC, string Subject, string Message, bool IsHTML)
        {
            MailMessage message = CreateMessage(From, To,  Subject, Message, IsHTML);

            if (CC != "")
            {
                foreach (String sCC in CC.Split(','))
                {
                    message.CC.Add(sCC);
                }
            }

            if (BCC != "")
            {
                foreach (String sBCC in BCC.Split(','))
                {
                    message.CC.Add(sBCC);
                }
            }

            return message;
        }

        public static MailMessage CreateMessage(string From, string To,  string Subject, string Message, bool IsHTML)
        {
            MailMessage message = new MailMessage();

            message.From = new MailAddress(From, "TeknoMobi"); ;

            // to remove issue with that as the server return error
            message.Subject = Subject.Replace(Environment.NewLine,";");

            foreach (String sTo in To.Split(','))
            {
                message.To.Add(sTo);
            }

            message.BodyEncoding = System.Text.Encoding.UTF8;

            //Body can be Html or text format
            //Specify true if it  is html message
            message.IsBodyHtml = IsHTML;

            // Message body content
            message.Body = Message;

            return message;
        }

        public static void Send(MailMessage message)
        {
            GetSmtpClient().Send(message);
        }

        public static SmtpClient GetSmtpClient()
        {
            return new teknoSmtpClient();
        }
       
    }

    internal class teknoSmtpClient : SmtpClient
    {
        public teknoSmtpClient()
        {
            Dictionary<string, string> smtpConfg = teknoSettings.Instance.SMTP ;

            Host = smtpConfg["host"];
            Port = int.Parse(smtpConfg["port"]);
            DeliveryMethod = SmtpDeliveryMethod.Network;
            UseDefaultCredentials = bool.Parse(smtpConfg["useDefaultCredentials"]);
            Credentials = new NetworkCredential(smtpConfg["userName"], smtpConfg["password"]);
            EnableSsl = bool.Parse(smtpConfg["enableSsl"]);
        }
    //        public new void  Send(MailMessage msg)
    //        {
    //            msg.Subject += "Laith New ";
    //            base.Send(msg);
    //        }

    //        public new void SendAsync(MailMessage msg, Object userToken)
    //        {
    //            base.SendAsync(msg, userToken);
    //        }
    }
}
