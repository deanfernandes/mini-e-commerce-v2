using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace EmailService;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public void SendConfirmationEmail(string toEmail, string token)
    {
        var smtpSection = _config.GetSection("SmtpSettings");
        var confirmBaseUrl = _config["AppSettings:ConfirmLinkBaseUrl"];
        var confirmLink = confirmBaseUrl + token;

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(smtpSection["Username"]));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Confirm your email";
        message.Body = new TextPart("html")
        {
            Text = $"<p>Thank you for registering!</p><p><a href=\"{confirmLink}\">Click here to confirm your email</a></p>"
        };

        using var smtp = new SmtpClient();
        smtp.Connect(smtpSection["Host"], int.Parse(smtpSection["Port"]), SecureSocketOptions.StartTls);
        smtp.Authenticate(smtpSection["Username"], smtpSection["Password"]);
        smtp.Send(message);
        smtp.Disconnect(true);

        Console.WriteLine($"Confirmation email sent to: {toEmail}");
    }
}