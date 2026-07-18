using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HospitalVacationManagement.Infrastructure.Data;

public sealed class DatabaseSeedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostEnvironment _environment;

    public DatabaseSeedService(
        IServiceProvider serviceProvider,
        IHostEnvironment environment)
    {
        _serviceProvider = serviceProvider;
        _environment = environment;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_environment.IsDevelopment())
        {
            return;
        }

        using var scope = _serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var adminAlreadyExists = await dbContext.Users
            .AnyAsync(user => user.Email == "admin@hospital.com", cancellationToken);

        if (adminAlreadyExists)
        {
            return;
        }

        var admin = new User(
            Guid.NewGuid(),
            "Administrador",
            "admin@hospital.com",
            passwordHasher.Hash("Admin@123"),
            UserRole.Admin);

        await dbContext.Users.AddAsync(admin, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}