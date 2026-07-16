using HospitalVacationManagement.Domain.Departments;
using HospitalVacationManagement.Domain.Employees;
using HospitalVacationManagement.Domain.Vacations;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
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