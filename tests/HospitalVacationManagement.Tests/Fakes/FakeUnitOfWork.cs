using HospitalVacationManagement.Application.Abstractions;

namespace HospitalVacationManagement.Tests.Fakes;

public sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveChangesCallCount { get; private set; }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCallCount++;

        return Task.CompletedTask;
    }
}