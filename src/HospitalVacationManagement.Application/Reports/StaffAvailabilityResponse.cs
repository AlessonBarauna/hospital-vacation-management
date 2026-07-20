namespace HospitalVacationManagement.Application.Reports;

public sealed record StaffAvailabilityResponse(
    Guid DepartmentId,
    int TotalEmployees,
    int EmployeesOnVacation,
    int AvailableEmployees,
    int AvailableSeniorEmployess
);