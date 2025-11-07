using System;

namespace project_z_backend.Interfaces.Services;

public interface IEmailTemplateService
{
    string GetEmailConfirmationTemplate(string username, string confirmationUrl);
}
