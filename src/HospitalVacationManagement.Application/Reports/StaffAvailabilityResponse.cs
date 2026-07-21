namespace HospitalVacationManagement.Application.Reports;

public sealed record StaffAvailabilityResponse(
    Guid DepartmentId,
    string DepartmentName,
    int TotalEmployees,
    int EmployeesOnVacation,
    int AvailableEmployees,
    int AvailableSeniorEmployees,
    bool IsSafe,
    string? RiskReason);