using HospitalVacationManagement.Domain.Employees;
using HospitalVacationManagement.Domain.Vacations;

namespace HospitalVacationManagement.Application.Vacations;

public sealed record VacationRequestSummaryResponse(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    Guid DepartmentId,
    string DepartmentName,
    JobRole Role,
    SeniorityLevel SeniorityLevel,
    DateOnly StartDate,
    DateOnly EndDate,
    VacationRequestStatus Status);