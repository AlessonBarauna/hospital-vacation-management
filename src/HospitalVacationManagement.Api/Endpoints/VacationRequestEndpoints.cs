using FluentValidation;
using HospitalVacationManagement.Application.Vacations;
using HospitalVacationManagement.Domain.Vacations;
using Microsoft.Extensions.Configuration.UserSecrets;
using HospitalVacationManagement.Application.Abstractions;
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
            ICurrentUserService currentUser,
            CancellationToken cancellationToken) =>
        {
            if (currentUser.UserId is not Guid userId)
            {
                return ApiErrors.Unauthorized();
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
            ICurrentUserService currentUser,
            ApproveVacationRequestHandler handler,
            CancellationToken cancellationToken) =>
        {
            if (currentUser.UserId is not Guid userId)
            {
                return ApiErrors.Unauthorized();
            }

            var response = await handler.HandleAsync(id, userId, cancellationToken);

            return response is null
    ? Results.NotFound()
    : Results.Ok(response);
        })
        .RequireAuthorization("ManagerOrAdmin");

        app.MapPut("/vacation-requests/{id:guid}/reject", async (
            Guid id,
            ICurrentUserService currentUser,
            RejectVacationRequestHandler handler,
            CancellationToken cancellationToken) =>
        {
            if (currentUser.UserId is not Guid userId)
            {
                return ApiErrors.Unauthorized();
            }

            var response = await handler.HandleAsync(id, userId, cancellationToken);

            return response is null
    ? Results.NotFound()
    : Results.Ok(response);
        })
        .RequireAuthorization("ManagerOrAdmin");

        app.MapPut("/vacation-requests/{id:guid}/cancel", async (
            Guid id,
            ICurrentUserService currentUser,
            [FromServices] GetVacationRequestByIdHandler getVacationRequestByIdHandler,
            [FromServices] CancelVacationRequestHandler cancelVacationRequestHandler,
            CancellationToken cancellationToken) =>
        {
            if (currentUser.UserId is not Guid userId)
            {
                return ApiErrors.Unauthorized();
            }

            var vacationRequest = await getVacationRequestByIdHandler.HandleAsync(id, cancellationToken);

            if (vacationRequest is null)
            {
                return ApiErrors.NotFound();
            }

            var userCanCancel =
                currentUser.Role is "Admin" or "Manager" ||
                vacationRequest.CreatedByUserId == userId;

            if (!userCanCancel)
            {
                return ApiErrors.Forbidden();
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