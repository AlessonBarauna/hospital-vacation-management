using HospitalVacationManagement.Domain.Employees;

namespace HospitalVacationManagement.Application.Employees;

public sealed record CreateEmployeeRequest(
    string FullName,
    Guid DepartmentId,
    JobRole Role,
    SeniorityLevel SeniorityLevel);