using FluentValidation;
using HospitalVacationManagement.Application.Users;
using System.Security.Claims;

namespace HospitalVacationManagement.Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/users", async (
            CreateUserRequest request,
            IValidator<CreateUserRequest> validator,
            CreateUserHandler handler,
            CancellationToken cancellationToken) =>
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var response = await handler.HandleAsync(request, cancellationToken);

            return Results.Created($"/users/{response.Id}", response);
        })
        .RequireAuthorization("AdminOnly");

        app.MapGet("/users", async (
            ListUsersHandler handler,
            CancellationToken cancellationToken) =>
        {
            var response = await handler.HandleAsync(cancellationToken);

            return Results.Ok(response);
        })
        .RequireAuthorization("AdminOnly");

        app.MapGet("/users/{id:guid}", async (
            Guid id,
            GetUserByIdHandler handler,
            CancellationToken cancellationToken) =>
        {
            var response = await handler.HandleAsync(id, cancellationToken);

            return response is null
                ? Results.NotFound()
                : Results.Ok(response);
        })
        .RequireAuthorization("AdminOnly");

        app.MapPut("/users/{id:guid}", async (
            Guid id,
            UpdateUserRequest request,
            IValidator<UpdateUserRequest> validator,
            UpdateUserHandler handler,
            CancellationToken cancellationToken) =>
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var response = await handler.HandleAsync(id, request, cancellationToken);

            return response is null
                ? Results.NotFound()
                : Results.Ok(response);
        })
        .RequireAuthorization("AdminOnly");

        app.MapPut("/users/{id:guid}/password", async (
            Guid id,
            ChangeUserPasswordRequest request,
            IValidator<ChangeUserPasswordRequest> validator,
            ChangeUserPasswordHandler handler,
            CancellationToken cancellationToken) =>
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var passwordWasChanged = await handler.HandleAsync(id, request, cancellationToken);

            return passwordWasChanged
                ? Results.NoContent()
                : Results.NotFound();
        })
        .RequireAuthorization("AdminOnly");

        app.MapPut("/users/{id:guid}/deactivate", async (
            Guid id,
            ClaimsPrincipal currentUser,
            DeactivateUserHandler deactivateUserHandler,
            CancellationToken cancellationToken) =>
        {
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId == id.ToString())
            {
                return Results.BadRequest("You cannot deactivate your own user.");
            }

            var userWasDeactivated = await deactivateUserHandler.HandleAsync(id, cancellationToken);

            return userWasDeactivated
                ? Results.NoContent()
                : Results.NotFound();
        })
        .RequireAuthorization("AdminOnly");

        app.MapPut("/users/{id:guid}/activate", async (
            Guid id,
            ActivateUserHandler handler,
            CancellationToken cancellationToken) =>
        {
            var userWasActivated = await handler.HandleAsync(id, cancellationToken);

            return userWasActivated
                ? Results.NoContent()
                : Results.NotFound();
        })
        .RequireAuthorization("AdminOnly");

        return app;
    }
}