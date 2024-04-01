using System.ComponentModel;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.SemanticKernel;
using MimeKit;

namespace ChatbotAI.net.Plugins
{
    public class GmailPlugin
    {
        [KernelFunction]
        [Description("Sends an email to a recipient.")]
        public async Task<string> SendEmailAsync(
            Kernel kernel,
            [Description("Semicolon delimatated list of emails of the recipients.")]
            string recipients,
            string topics,
            string body)
        {
            try
            {

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Plusware AI", "plusware.ai@gmail.com"));
                foreach (var recipient in recipients.Split(';'))
                {
                    message.To.Add(new MailboxAddress(recipient, recipient));
                }
                message.Subject = topics;
                message.Body = new TextPart(body);


                using (var smtpClient = new SmtpClient())
                {
                    await smtpClient.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                    await smtpClient.AuthenticateAsync("plusware.ai@gmail.com", "wcfg rzop svea gzjl");
                    await smtpClient.SendAsync(message);
                    await smtpClient.DisconnectAsync(true);
                }
                return Task.FromResult("The email was sent successfully").Result;
            }
            catch (Exception e)
            {
                return Task.FromResult("The email could not be sent").Result;
            }
        }
    }
}
