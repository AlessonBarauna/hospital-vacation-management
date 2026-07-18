using HospitalVacationManagement.Domain.Users;

namespace HospitalVacationManagement.Application.Users;

public sealed record UpdateUserRequest(
    string FullName,
    string Email,
    UserRole Role);