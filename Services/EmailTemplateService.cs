using System;
using project_z_backend.Interfaces.Services;

namespace project_z_backend.Services;

public class EmailTemplateService : IEmailTemplateService
{
    private readonly IConfiguration _config;
    public EmailTemplateService(IConfiguration configuration)
    {
        _config = configuration;
    }
    
    public string GetEmailConfirmationTemplate(string username, string confirmationUrl)
    {
        var appName = _config["App:Name"];
        var supportEmail = _config["App:SupportEmail"];

        return $@"<!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>{appName}: Xác nhận Email của bạn</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #28a745; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 20px; background-color: #f9f9f9; }}
                        .button {{ display: inline-block; padding: 12px 24px; background-color: #28a745; color: white; text-decoration: none; border-radius: 4px; margin: 20px 0; }}
                        .footer {{ padding: 20px; text-align: center; font-size: 12px; color: #666; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Chào mừng đến với {appName}!</h1>
                        </div>
                        <div class='content'>
                            <h2>Xác nhận địa chỉ Email của bạn</h2>
                            <p>Xin chào <strong>{username}</strong>,</p>
                            <p>Cảm ơn bạn đã tham gia {appName}! Vui lòng xác nhận địa chỉ email của bạn bằng cách nhấp vào nút bên dưới:</p>
                            <p style='text-align: center;'>
                                <a href='{confirmationUrl}' class='button'>Xác nhận Email</a>
                            </p>
                            <p>Nếu bạn không tạo tài khoản, bạn có thể bỏ qua email này.</p>
                        </div>
                        <div class='footer'>
                            <p>Email này được gửi bởi {appName}</p>
                            <p>Liên hệ với tôi tại <a href='mailto:{supportEmail}'>{supportEmail}</a></p>
                        </div>
                    </div>
                </body>
                </html>";
    }
}