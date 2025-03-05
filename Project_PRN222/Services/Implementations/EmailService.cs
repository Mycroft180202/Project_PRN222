using MailKit.Net.Smtp;
using MimeKit;
using Project_PRN222.Services.Interfaces;

namespace Project_PRN222.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_configuration["Email:SenderName"],
                                            _configuration["Email:SenderEmail"]));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = subject;
            email.Body = new TextPart("plain") { Text = message };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_configuration["Email:SmtpHost"],
                                  int.Parse(_configuration["Email:SmtpPort"]),
                                  MailKit.Security.SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(_configuration["Email:Username"],
                                       _configuration["Email:Password"]);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
