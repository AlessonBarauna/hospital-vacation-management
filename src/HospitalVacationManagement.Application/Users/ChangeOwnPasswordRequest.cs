namespace HospitalVacationManagement.Application.Users;

public sealed record ChangeOwnPasswordRequest(
    string CurrentPassword,
    string NewPassword);