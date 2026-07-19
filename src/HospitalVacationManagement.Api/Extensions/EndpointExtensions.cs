using HospitalVacationManagement.Api.Endpoints;

namespace HospitalVacationManagement.Api.Extensions;

public static class EndpointExtensions
{
    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        app.MapSystemEndpoints();
        app.MapAuthEndpoints();
        app.MapUserEndpoints();
        app.MapMeEndpoints();
        app.MapDepartmentEndpoints();
        app.MapEmployeeEndpoints();
        app.MapVacationRequestEndpoints();

        return app;
    }
}