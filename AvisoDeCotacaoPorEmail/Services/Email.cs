using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace AvisoDeCotacaoPorEmail
{
    public class Email
    {
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                 throw new Exception("Email failed to send :: " + e.Error.ToString());
            } 
        }

        private static IConfigurationRoot GetConfig()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("smtpSettings.json");
            var config = builder.Build();
            return config;
        }
        private static SmtpClient GetClient()
        {
            var config = GetConfig();

            return new SmtpClient(config["Smtp:Host"])
            {
                Port = int.Parse(config["Smtp:Port"]),
                Credentials = new NetworkCredential(config["Smtp:Email"], config["Smtp:Password"]),
                EnableSsl = true,
            }; 
        }

        public async Task SendEmail(String toStr, String messageStr, String subjectStr)
        {
            var client = GetClient();
            client.SendCompleted += new
                SendCompletedEventHandler(SendCompletedCallback);
            MailAddress from = new MailAddress(GetConfig()["Smtp:Email"],
                GetConfig()["Smtp:DisplayName"],
                System.Text.Encoding.UTF8);
            MailAddress to = new MailAddress(toStr);
            MailMessage message = new MailMessage(from, to);

            message.Body = messageStr;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Subject = subjectStr;
            message.SubjectEncoding = System.Text.Encoding.UTF8;

            // The userState can be any object that allows your callback
            // method to identify this send operation.
            // For this example, the userToken is a string constant.
            string userState = messageStr;
            await client.SendMailAsync(message);
            Console.WriteLine("Message sent");
            message.Dispose();

        }
    }
}