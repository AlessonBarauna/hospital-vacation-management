using HospitalVacationManagement.Application.Abstractions;

namespace HospitalVacationManagement.Tests.Fakes;

public sealed class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return $"hashed:{password}";
    }

    public bool Verify(string password, string passwordHash)
    {
        return passwordHash == $"hashed:{password}";
    }
}