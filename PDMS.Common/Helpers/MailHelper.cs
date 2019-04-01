using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Common.Helpers
{
    public static class MailHelper
    {
        public static void SendMail(List<string> To, List<string> CC, List<string> BCC, string Subject, string Body, List<string> Attachments)
        {
            SmtpClient mySmtp = new SmtpClient("CORIMC04.corp.jabil.org");

            MailMessage msgMail = new MailMessage();

            foreach (string Receiver in To)
            {
                if (!String.IsNullOrEmpty(Receiver))
                    msgMail.To.Add(Receiver.Trim());
            }

            if (CC != null && CC.Count() > 0)
            {
                foreach (string Receiver in CC)
                {
                    msgMail.CC.Add(Receiver.Trim());
                }
            }

            if (BCC != null && BCC.Count() > 0)
            {
                foreach (string Receiver in BCC)
                {
                    msgMail.Bcc.Add(Receiver.Trim());
                }
            }

            //Sender email, name
            //msgMail.From = new MailAddress("do-not-reply@jabil.com", "Report System & Regional System");
            msgMail.From = new MailAddress("do-not-reply@jabil.com", "PIS Remind");
            msgMail.Subject = Subject;
            msgMail.SubjectEncoding = System.Text.Encoding.UTF8;

            foreach (string FilePath in Attachments)
            {
                if (File.Exists(FilePath))
                {
                    Attachment attachment = new Attachment(FilePath);
                    msgMail.Attachments.Add(attachment);
                }
            }

            msgMail.Body = Body;
            msgMail.BodyEncoding = System.Text.Encoding.UTF8;
            msgMail.IsBodyHtml = true;

            mySmtp.Send(msgMail);
        }
    }
}
