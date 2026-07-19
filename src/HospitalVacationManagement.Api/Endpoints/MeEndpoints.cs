using FluentValidation;
using HospitalVacationManagement.Application.Users;
using System.Security.Claims;

namespace HospitalVacationManagement.Api.Endpoints;

public static class MeEndpoints
{
    public static IEndpointRouteBuilder MapMeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/me", async (
            ClaimsPrincipal currentUser,
            GetCurrentUserHandler handler,
            CancellationToken cancellationToken) =>
        {
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(currentUserId, out var userId))
            {
                return Results.Unauthorized();
            }

            var response = await handler.HandleAsync(userId, cancellationToken);

            return response is null
                ? Results.NotFound()
                : Results.Ok(response);
        })
        .RequireAuthorization();

        app.MapPut("/me/password", async (
            ClaimsPrincipal currentUser,
            ChangeOwnPasswordRequest request,
            IValidator<ChangeOwnPasswordRequest> validator,
            ChangeOwnPasswordHandler handler,
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
                ChangeOwnPasswordResult.InvalidCurrentPassword => Results.BadRequest("Current password is invalid."),
                _ => Results.BadRequest()
            };
        })
        .RequireAuthorization();

        return app;
    }
}