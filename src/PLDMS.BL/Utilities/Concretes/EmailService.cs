using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace PLDMS.BL.Utilities;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = body };
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync(
                emailSettings["SmtpServer"], 
                int.Parse(emailSettings["SmtpPort"]), 
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(emailSettings["SenderEmail"], emailSettings["AppPassword"]);
            
            await smtp.SendAsync(email);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Email cloud not be sent.", ex);
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }
}