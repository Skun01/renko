using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using project_z_backend.Interfaces;
using project_z_backend.Interfaces.Services;
using project_z_backend.Options;
using project_z_backend.Share;

namespace project_z_backend.Services;

public class EmailSenderService : IEmailSenderService
{
    private readonly EmailSettings _emailSettings;
    private readonly AppSettings _appSettings;
    private readonly FrontendSettings _frontendSettings;
    private readonly IEmailTemplateService _emailTemplate;
    public EmailSenderService(
        IOptions<EmailSettings> emailSettings,
        IOptions<AppSettings> appSettings,
        IOptions<FrontendSettings> frontendSettings,
        IEmailTemplateService emailTemplateService)
    {
        _emailSettings = emailSettings.Value;
        _appSettings = appSettings.Value;
        _frontendSettings = frontendSettings.Value;
        _emailTemplate = emailTemplateService;
    }
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
        {
            Credentials = new NetworkCredential(_emailSettings.FromAddress, _emailSettings.Password),
            EnableSsl = true
        };
        await client.SendMailAsync(
            new MailMessage(_emailSettings.FromAddress, email, subject, htmlMessage) { IsBodyHtml = true }
        );
    }

    public async Task<Result> SendVerifyEmailAsync(string username, string userEmail, string verifyEmailToken)
    {
        try
        {
            string callbackUrl = $"{_frontendSettings.BaseUrl}{_frontendSettings.VerifyEmailPath}?token={verifyEmailToken}";
            string emailTemplate = _emailTemplate.GetEmailConfirmationTemplate(
                username,
                callbackUrl
            );
            await SendEmailAsync(userEmail, $"[{_appSettings.Name}]: Confirm your email", emailTemplate);
            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure(Error.InternalError("Cannot send verify email"));
        }
        
    }
}
