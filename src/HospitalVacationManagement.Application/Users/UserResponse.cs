using HospitalVacationManagement.Domain.Users;

namespace HospitalVacationManagement.Application.Users;

public sealed record UserResponse(
    Guid Id,
    string FullName,
    string Email,
    UserRole Role);