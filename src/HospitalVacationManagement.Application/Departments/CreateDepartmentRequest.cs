namespace HospitalVacationManagement.Application.Departments;

public sealed record CreateDepartmentRequest(
    string Name,
    int MaximumSimultaneousVacations);