using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Text;
using Tasker.Core.Interfaces.Services;

namespace Tasker.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailWithCsvBodyAsync(string recipientEmail, string subject, string csvString)
    {
        var bodyBuilder = new BodyBuilder();
        
        bodyBuilder.Attachments.Add("data.csv", Encoding.ASCII.GetBytes(csvString), new ContentType("text", "csv"));

        var message = new MimeMessage();
        
        message.From.Add(new MailboxAddress(_config["Email:Name"], _config["Email:Email"]));
        message.To.Add(MailboxAddress.Parse(recipientEmail));
        message.Subject = subject;
        message.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(_config["Email:Server"], int.Parse(_config["Email:Port"]), SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_config["Email:Email"], _config["Email:Password"]);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}
