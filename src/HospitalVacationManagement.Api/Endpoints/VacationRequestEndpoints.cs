using FluentValidation;
using HospitalVacationManagement.Application.Vacations;
using HospitalVacationManagement.Domain.Vacations;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Security.Claims;
using HospitalVacationManagement.Api.Errors;
using Microsoft.AspNetCore.Mvc;

namespace HospitalVacationManagement.Api.Endpoints;

public static class VacationRequestEndpoints
{
    public static IEndpointRouteBuilder MapVacationRequestEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/vacation-requests/validate", async (
            ValidateVacationRequest request,
            IValidator<ValidateVacationRequest> validator,
            [FromServices] ValidateVacationRequestHandler handler,
            CancellationToken cancellationToken) =>
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var response = await handler.HandleAsync(request, cancellationToken);

            return Results.Ok(response);
        })
        .RequireAuthorization();

        app.MapPost("/vacation-requests", async (
            RequestVacationRequest request,
            IValidator<RequestVacationRequest> validator,
            RequestVacationHandler handler,
            ClaimsPrincipal currentUser,
            CancellationToken cancellationToken) =>
        {
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(currentUserId, out var userId))
            {
                return Results.Unauthorized();
            }

            var response = await handler.HandleAsync(request, userId, cancellationToken);

            return response.IsValid
                ? Results.Created($"/vacation-requests/{response.VacationRequestId}", response)
                : Results.BadRequest(response);
        })
        .RequireAuthorization();

        app.MapGet("/vacation-requests", async (
    VacationRequestStatus? status,
    Guid? employeeId,
    Guid? departmentId,
    DateOnly? startDate,
    DateOnly? endDate,
    int page,
    int pageSize,
    ListVacationRequestsHandler handler,
    CancellationToken cancellationToken) =>
{
    var request = new ListVacationRequestsRequest(
        status,
        employeeId,
        departmentId,
        startDate,
        endDate,
        page,
        pageSize);

    var response = await handler.HandleAsync(request, cancellationToken);

    return Results.Ok(response);
})
.RequireAuthorization();

        app.MapGet("/vacation-requests/{id:guid}", async (
            Guid id,
            GetVacationRequestByIdHandler handler,
            CancellationToken cancellationToken) =>
        {
            var response = await handler.HandleAsync(id, cancellationToken);

            return response is null
                ? Results.NotFound()
                : Results.Ok(response);
        })
        .RequireAuthorization();

        app.MapPut("/vacation-requests/{id:guid}/approve", async (
            Guid id,
            ClaimsPrincipal currentUser,
            ApproveVacationRequestHandler handler,
            CancellationToken cancellationToken) =>
        {
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(currentUserId, out var userId))
            {
                return Results.Unauthorized();
            }

            var response = await handler.HandleAsync(id, userId, cancellationToken);

            return response is null
    ? Results.NotFound()
    : Results.Ok(response);
        })
        .RequireAuthorization("ManagerOrAdmin");

        app.MapPut("/vacation-requests/{id:guid}/reject", async (
            Guid id,
            ClaimsPrincipal currentUser,
            RejectVacationRequestHandler handler,
            CancellationToken cancellationToken) =>
        {
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(currentUserId, out var userId))
            {
                return Results.Unauthorized();
            }

            var response = await handler.HandleAsync(id, userId, cancellationToken);

            return response is null
    ? Results.NotFound()
    : Results.Ok(response);
        })
        .RequireAuthorization("ManagerOrAdmin");

        app.MapPut("/vacation-requests/{id:guid}/cancel", async (
            Guid id,
            ClaimsPrincipal currentUser,
            GetVacationRequestByIdHandler getVacationRequestByIdHandler,
            CancelVacationRequestHandler cancelVacationRequestHandler,
            CancellationToken cancellationToken) =>
        {
            var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(currentUserId, out var userId))
            {
                return ApiErrors.Unauthorized();
            }

            var currentUserRole = currentUser.FindFirstValue(ClaimTypes.Role);

            var vacationRequest = await getVacationRequestByIdHandler.HandleAsync(id, cancellationToken);

            if (vacationRequest is null)
            {
                return ApiErrors.NotFound();
            }

            var userIsManagerOrAdmin =
                currentUserRole == "Admin" ||
                currentUserRole == "Manager";

            var userCreatedVacationRequest =
                vacationRequest.CreatedByUserId == userId;

            if (!userIsManagerOrAdmin && !userCreatedVacationRequest)
            {
                return ApiErrors.Forbidden("You can only cancel your own vacation requests.");
            }

            var response = await cancelVacationRequestHandler.HandleAsync(id, userId, cancellationToken);

            return response is null
                ? ApiErrors.NotFound()
                : Results.Ok(response);
        })
        .RequireAuthorization();

        return app;
    }
}