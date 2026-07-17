namespace HospitalVacationManagement.Application.Authentication;

public interface IJwtTokenGenerator
{
    LoginResponse Generate(string email, string role);
}