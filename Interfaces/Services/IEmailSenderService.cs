using project_z_backend.Share;

namespace project_z_backend.Interfaces;

public interface IEmailSenderService
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
    Task<Result> SendVerifyEmailAsync(string username, string userEmail, string callbackUrl);
}
