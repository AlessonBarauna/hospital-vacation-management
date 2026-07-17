using HospitalVacationManagement.Domain.Employees;

namespace HospitalVacationManagement.Application.Employees;

public sealed record EmployeeResponse(
    Guid Id,
    string FullName,
    Guid DepartmentId,
    JobRole Role,
    SeniorityLevel SeniorityLevel);