using System;
using System.Net.Mail;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Microsoft.Extensions.Logging;

namespace QueueTrigger
{
    public static class QueueTrigger
    {
       

        [FunctionName("QueueTrigger")]
        public static void Run([QueueTrigger("capstone6", Connection = "AzureConnString")]string myQueueItem, ILogger log)
        {

            try
            {
                string from = Environment.GetEnvironmentVariable("FromEmail");
                string to = Environment.GetEnvironmentVariable("ToEmail");
                string username = Environment.GetEnvironmentVariable("SmtpUsername");
                string password = Environment.GetEnvironmentVariable("SmtpPassword");
                string smtpserver = Environment.GetEnvironmentVariable("SmtpServer");
                int port = Convert.ToInt32(Environment.GetEnvironmentVariable("SmtpPort"));

                MailAddress mailAddress = new MailAddress(to);
                MailMessage message = new MailMessage(from, to);
                message.From = new MailAddress(from);
                message.To.Add(to);
                message.IsBodyHtml = false;
                message.Subject = "New System Activities";
                message.Body = myQueueItem;

                SmtpClient smtpClient = new SmtpClient(smtpserver, port);
                smtpClient.Credentials = new System.Net.NetworkCredential(username, password);
                smtpClient.EnableSsl = true;

                smtpClient.Send(message);

                log.LogInformation("Mail sent successfully");
                log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Code has some error : {ex.Message}");
            }
            
        }
    }
    
}
