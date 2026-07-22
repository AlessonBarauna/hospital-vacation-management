using FluentValidation;
using HospitalVacationManagement.Api.Errors;
using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Application.Users;
using Microsoft.AspNetCore.Mvc;

namespace HospitalVacationManagement.Api.Endpoints;

public static class MeEndpoints
{
    public static IEndpointRouteBuilder MapMeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/me", async (
            ICurrentUserService currentUser,
            [FromServices] GetCurrentUserHandler handler,
            CancellationToken cancellationToken) =>
        {
            if (currentUser.UserId is not Guid userId)
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
            ICurrentUserService currentUser,
            ChangeOwnPasswordRequest request,
            IValidator<ChangeOwnPasswordRequest> validator,
            [FromServices] ChangeOwnPasswordHandler handler,
            CancellationToken cancellationToken) =>
        {
            if (currentUser.UserId is not Guid userId)
            {
                return ApiErrors.Unauthorized();
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