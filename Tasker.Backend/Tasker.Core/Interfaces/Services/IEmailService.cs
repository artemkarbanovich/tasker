namespace Tasker.Core.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailWithCsvBodyAsync(string recipientEmail, string subject, string csvString);
}
