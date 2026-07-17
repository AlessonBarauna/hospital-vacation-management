namespace HospitalVacationManagement.Application.Departments;

public sealed record DepartmentResponse(
    Guid Id,
    string Name,
    int MaximumSimultaneousVacations);