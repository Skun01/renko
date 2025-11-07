using System;
using project_z_backend.Entities;

namespace project_z_backend.Interfaces.Services;

public interface ITokenService
{
    string CreateAccessToken(User user);
    string CreateEmailConfirmationToken(User user);
}


