using System;

namespace project_z_backend.Share;

// Generic 
public class ApiResponse<T>
{
    public bool Success { set; get; }
    public List<string>? Errors { set; get; }
    public T? Data { set; get; }
    public string? Message { set; get; }
    public int StatusCode { set; get; }
    public DateTime Timestamp { set; get; } = DateTime.UtcNow;
    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            StatusCode = 200
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null, int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors,
            StatusCode = statusCode
        };
    }
}

// Non-generic : API response with no data
public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? TraceId { get; set; }
    public int? StatusCode { get; set; }

    public static ApiResponse SuccessResponse(string? message = null)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            StatusCode = 200
        };
    }

    public static ApiResponse ErrorResponse(string message, List<string>? errors = null, int statusCode = 400)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors,
            StatusCode = statusCode
        };
    }
}
