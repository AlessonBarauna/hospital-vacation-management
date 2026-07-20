namespace HospitalVacationManagement.Application.Reports;

public sealed record VacationsByDepartmentRequest(
    int Year,
    int Month);