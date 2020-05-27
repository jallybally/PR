using System;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace pgbackup
{
    class Email
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("1C Backup", Function.SendEmail));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            try
            {
                emailMessage.Body = new TextPart("Plain")
                {
                    Text = message
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate(Function.SendEmail, Function.SendEmailPass);
                client.Send(emailMessage);
                client.Disconnect(true);
            }
        }
    }
}
