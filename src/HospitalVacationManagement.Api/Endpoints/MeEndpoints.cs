using FluentValidation;
using HospitalVacationManagement.Application.Users;
using System.Security.Claims;
using HospitalVacationManagement.Api.Errors;
using Microsoft.AspNetCore.Mvc;

namespace HospitalVacationManagement.Api.Endpoints;

public static class MeEndpoints
{
    public static IEndpointRouteBuilder MapMeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/me", async (
            ClaimsPrincipal currentUser,
            [FromServices] GetCurrentUserHandler handler,
            CancellationToken cancellationToken) =>
        {
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(currentUserId, out var userId))
            {
                return ApiErrors.Unauthorized();
            }

            var response = await handler.HandleAsync(userId, cancellationToken);

            return response is null
                ? ApiErrors.NotFound()
                : Results.Ok(response);
        })
        .RequireAuthorization();

        app.MapPut("/me/password", async (
            ClaimsPrincipal currentUser,
            ChangeOwnPasswordRequest request,
            IValidator<ChangeOwnPasswordRequest> validator,
            [FromServices] ChangeOwnPasswordHandler handler,
            CancellationToken cancellationToken) =>
        {
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(currentUserId, out var userId))
            {
                return Results.Unauthorized();
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var result = await handler.HandleAsync(userId, request, cancellationToken);

            return result switch
            {
                ChangeOwnPasswordResult.Success => Results.NoContent(),
                ChangeOwnPasswordResult.UserNotFound => Results.NotFound(),
                ChangeOwnPasswordResult.InvalidCurrentPassword => ApiErrors.BadRequest("Current password is invalid."),
                _ => Results.BadRequest()
            };
        })
        .RequireAuthorization();

        return app;
    }
}