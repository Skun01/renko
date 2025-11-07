namespace project_z_backend.Interfaces;

public interface IEmailSenderService
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}
