namespace HospitalVacationManagement.Api.Endpoints;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        var apiV1 = app.MapGroup("/api/v1");

        apiV1.MapAuthEndpoints();
        apiV1.MapMeEndpoints();
        apiV1.MapUserEndpoints();
        apiV1.MapDepartmentEndpoints();
        apiV1.MapEmployeeEndpoints();
        apiV1.MapVacationRequestEndpoints();
        apiV1.MapCalendarEndpoints();
        apiV1.MapReportEndpoints();

        app.MapSystemEndpoints();

        return app;
    }
}