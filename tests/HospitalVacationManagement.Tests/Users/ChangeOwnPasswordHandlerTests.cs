using HospitalVacationManagement.Application.Users;
using HospitalVacationManagement.Domain.Users;
using HospitalVacationManagement.Tests.Fakes;
using Xunit;

namespace HospitalVacationManagement.Tests.Users;

public sealed class ChangeOwnPasswordHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnInvalidCurrentPassword_WhenCurrentPasswordIsWrong()
    {
        var userRepository = new FakeUserRepository();
        var passwordHasher = new FakePasswordHasher();
        var unitOfWork = new FakeUnitOfWork();

        var user = new User(
            Guid.NewGuid(),
            "Maria Gestora",
            "maria.gestora@hospital.com",
            passwordHasher.Hash("SenhaAtual@123"),
            UserRole.Manager);

        await userRepository.AddAsync(user);

        var handler = new ChangeOwnPasswordHandler(
            userRepository,
            passwordHasher,
            unitOfWork);

        var request = new ChangeOwnPasswordRequest(
            "SenhaErrada@123",
            "NovaSenha@123");

        var result = await handler.HandleAsync(user.Id, request);

        Assert.Equal(ChangeOwnPasswordResult.InvalidCurrentPassword, result);
        Assert.True(passwordHasher.Verify("SenhaAtual@123", user.PasswordHash));
    }
}