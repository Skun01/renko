using System;
using project_z_backend.Share;

namespace project_z_backend.Mapping;

public static class ResultMapping
{
    public static IResult ToApiResponse(this Result result, string? successMessage)
    {
        // when success
        if (result.IsSuccess)
        {
            var response = ApiResponse.SuccessResponse(successMessage);
            return Results.Ok(response);
        }

        // when error
        var errorResponse = ApiResponse.ErrorResponse(
            result.Error!.Description,
            null,
            int.Parse(result.Error.Code)
        );

        return Results.Json(errorResponse, statusCode: errorResponse.StatusCode ?? 400);
    }

    public static IResult ToApiResponse<T>(this Result<T> result, string? successMessage)
    {
        if (result.IsSuccess)
        {
            var response = ApiResponse<T>.SuccessResponse(result.Value!, successMessage);
            return Results.Ok(response);
        }

        var errorResponse = ApiResponse<T>.ErrorResponse(
            result.Error!.Description,
            null,
            int.Parse(result.Error.Code)
        );

        return Results.Json(errorResponse, statusCode: errorResponse.StatusCode ?? 400);
    }
}
