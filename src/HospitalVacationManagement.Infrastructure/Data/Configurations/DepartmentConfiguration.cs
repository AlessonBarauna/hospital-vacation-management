using HospitalVacationManagement.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalVacationManagement.Infrastructure.Data.Configurations;

public sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder.HasKey(department => department.Id);

        builder.Property(department => department.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(department => department.MaximumSimultaneousVacations)
            .IsRequired();
    }
}