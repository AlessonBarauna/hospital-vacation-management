using HospitalVacationManagement.Domain.Users;

namespace HospitalVacationManagement.Application.Users;

public sealed record CreateUserRequest(
    string FullName,
    string Email,
    string Password,
    UserRole Role);