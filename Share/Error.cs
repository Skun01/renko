using System;

namespace project_z_backend.Share;

public record class Error(string Code, string Description)
{
    public static Error NotFound(string message) =>
        new Error("404", message);

    public static Error Conflict(string message) =>
        new Error("409", message);

    public static Error BadRequest(string message) =>
        new Error("400", message);

    public static Error InternalError(string message) =>
        new Error("500", message);

    public static Error Unauthorized(string message) =>
        new Error("401", message);
}
