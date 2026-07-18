namespace HospitalVacationManagement.Application.Users;

public sealed record ChangeUserPasswordRequest(
    string NewPassword);