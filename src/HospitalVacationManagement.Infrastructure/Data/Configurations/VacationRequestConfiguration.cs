using HospitalVacationManagement.Domain.Vacations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalVacationManagement.Infrastructure.Data.Configurations;

public sealed class VacationRequestConfiguration : IEntityTypeConfiguration<VacationRequest>
{
    public void Configure(EntityTypeBuilder<VacationRequest> builder)
    {
        builder.ToTable("vacation_requests");

        builder.HasKey(vacationRequest => vacationRequest.Id);

        builder.Property(vacationRequest => vacationRequest.EmployeeId)
            .IsRequired();

        builder.Property(vacationRequest => vacationRequest.StartDate)
            .IsRequired();

        builder.Property(vacationRequest => vacationRequest.EndDate)
            .IsRequired();

        builder.Property(vacationRequest => vacationRequest.Status)
            .IsRequired();
        builder.Property(vacationRequest => vacationRequest.CreatedAt)
            .IsRequired();

        builder.Property(vacationRequest => vacationRequest.UpdatedAt);

        builder.Property(vacationRequest => vacationRequest.ApprovedByUserId);

        builder.Property(vacationRequest => vacationRequest.RejectedByUserId);

        builder.Property(vacationRequest => vacationRequest.CancelledByUserId);
    }
}