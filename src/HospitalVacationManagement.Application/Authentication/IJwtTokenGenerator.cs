namespace HospitalVacationManagement.Application.Authentication;
using System;

public interface IJwtTokenGenerator
{
    LoginResponse Generate(Guid userId, string email, string role);
}