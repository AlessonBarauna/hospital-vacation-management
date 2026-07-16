using HospitalVacationManagement.Domain.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalVacationManagement.Infrastructure.Data.Configurations;

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(employee => employee.Id);

        builder.Property(employee => employee.FullName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(employee => employee.DepartmentId)
            .IsRequired();

        builder.Property(employee => employee.Role)
            .IsRequired();

        builder.Property(employee => employee.SeniorityLevel)
            .IsRequired();

    }
}