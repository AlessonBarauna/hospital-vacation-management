namespace HospitalVacationManagement.Application.Authentication;

public sealed record LoginResponse(string AccessToken, DateTime ExpiresAt);