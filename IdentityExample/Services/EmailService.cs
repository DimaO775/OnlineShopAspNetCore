using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace IdentityExample.Services
{
    public interface IEmailService
    {
        Task SendAsync(string from, string to, string subject, string html);
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class SmtpHiddenInfo
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public int SecureSocketOptions { get; set; }

        public string User { get; set; }

        public string Password { get; set; }
    }
    public class EmailService : IEmailService
    {
        public  EmailService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public async Task SendAsync(string from, string to, string subject, string html)
        {
            var email = new MimeMessage();
            //email.From.Add(new MailboxAddress("Sender", from));
            email.From.Add(new MailboxAddress("Администрация сайта", "dima.orlov2016@gmail.com"));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };
            SmtpHiddenInfo smtpHiddenInfo = new SmtpHiddenInfo();
            Configuration.GetSection("SmtpHiddenInfo").Bind(smtpHiddenInfo);
            using var smtp = new SmtpClient();
            
            await smtp.ConnectAsync(smtpHiddenInfo.Host, smtpHiddenInfo.Port, (SecureSocketOptions)smtpHiddenInfo.SecureSocketOptions);

            await smtp.AuthenticateAsync(smtpHiddenInfo.User, "qlhetyzgotypnsqb");
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }


        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Техническая поддержка интернет магазина \"Абобус\"", "dima.orlov2016@gmail.com"));
            emailMessage.To.Add(MailboxAddress.Parse(email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };
            

            using (var client = new SmtpClient())
            {
                
                await client.ConnectAsync("smtp.gmail.com", 587);
                await client.AuthenticateAsync(Configuration["SecretKeyForConfirmRegister"], Configuration["SecretValueForConfirmRegister"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
                
            }
        }
    }
}
