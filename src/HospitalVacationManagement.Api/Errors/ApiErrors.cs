using Microsoft.AspNetCore.Mvc;

namespace HospitalVacationManagement.Api.Errors;

public static class ApiErrors
{
    public static IResult BadRequest(string detail)
    {
        return Results.Problem(
            title: "Bad Request",
            detail: detail,
            statusCode: StatusCodes.Status400BadRequest,
            type: "https://httpstatuses.com/400");
    }

    public static IResult Unauthorized(string detail = "Authentication is required.")
    {
        return Results.Problem(
            title: "Unauthorized",
            detail: detail,
            statusCode: StatusCodes.Status401Unauthorized,
            type: "https://httpstatuses.com/401");
    }

    public static IResult Forbidden(string detail = "You do not have permission to access this resource.")
    {
        return Results.Problem(
            title: "Forbidden",
            detail: detail,
            statusCode: StatusCodes.Status403Forbidden,
            type: "https://httpstatuses.com/403");
    }

    public static IResult NotFound(string detail = "The requested resource was not found.")
    {
        return Results.Problem(
            title: "Not Found",
            detail: detail,
            statusCode: StatusCodes.Status404NotFound,
            type: "https://httpstatuses.com/404");
    }

    public static IResult Conflict(string detail)
    {
        return Results.Problem(
            title: "Conflict",
            detail: detail,
            statusCode: StatusCodes.Status409Conflict,
            type: "https://httpstatuses.com/409");
    }
}