using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace PDMS.Web.Tasks
{
    public class TaskMessageMail : IPISTaskBase
    {
        [AutomaticRetry(Attempts = 0)]
        public static void MailSendTest(string toWho, string subject, string body, IJobCancellationToken cancellationToken)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "corimc04.corp.jabil.org";
            client.Port = 25;
            client.EnableSsl = false;
            for (int i = 0; i < 5; i++)
            {
                // cancel job if cancelled
                cancellationToken.ThrowIfCancellationRequested();
                // start job content
                MailMessage message = new MailMessage();
                try
                {
                    message.To.Add(toWho);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;
                    client.Send(message);
                }
                catch
                {
                    System.Threading.Thread.Sleep(TimeSpan.FromMinutes(5));
                }
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public static void MailSend(string toWho, string subject, string body, IJobCancellationToken cancellationToken)
        {

        }
    }
}