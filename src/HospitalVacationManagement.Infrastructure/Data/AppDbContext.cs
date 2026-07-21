using HospitalVacationManagement.Domain.Departments;
using HospitalVacationManagement.Domain.Employees;
using HospitalVacationManagement.Domain.Vacations;
using HospitalVacationManagement.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace HospitalVacationManagement.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<VacationRequest> VacationRequests => Set<VacationRequest>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>(builder =>
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(refreshToken => refreshToken.Id);

        builder.Property(refreshToken => refreshToken.Token)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(refreshToken => refreshToken.UserId)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.ExpiresAt)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.CreatedAt)
            .IsRequired();

        builder.HasIndex(refreshToken => refreshToken.Token)
            .IsUnique();

        builder.HasIndex(refreshToken => refreshToken.UserId);
    });
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        var emergencyDepartmentId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var karolId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var anaId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

        modelBuilder.Entity<Department>().HasData(
            new Department(
                emergencyDepartmentId,
                "Emergency",
                maximumSimultaneousVacations: 1));

        modelBuilder.Entity<Employee>().HasData(
            new Employee(
                karolId,
                "Karol Barauna",
                emergencyDepartmentId,
                JobRole.Nurse,
                SeniorityLevel.Junior),
            new Employee(
                anaId,
                "Ana Silva",
                emergencyDepartmentId,
                JobRole.Nurse,
                SeniorityLevel.Senior));
    }
}