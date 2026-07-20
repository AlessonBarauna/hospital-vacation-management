using HospitalVacationManagement.Domain.Vacations;
using Xunit;

namespace HospitalVacationManagement.Tests;

public sealed class VacationRequestTests
{
    [Fact]
    public void Approve_ShouldChangeStatusToApproved_WhenVacationRequestIsPending()
    {
        var vacationRequest = new VacationRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 7, 10),
            new DateOnly(2026, 7, 20),
            VacationRequestStatus.Pending);

        var approvedByUserId = Guid.NewGuid();

        vacationRequest.Approve(approvedByUserId);

        Assert.Equal(VacationRequestStatus.Approved, vacationRequest.Status);
        Assert.Equal(approvedByUserId, vacationRequest.ApprovedByUserId);
        Assert.NotNull(vacationRequest.UpdatedAt);
    }

    [Fact]
    public void Reject_ShouldChangeStatusToRejected_WhenVacationRequestIsPending()
    {
        var vacationRequest = new VacationRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 7, 10),
            new DateOnly(2026, 7, 20),
            VacationRequestStatus.Pending);

        var rejectedByUserId = Guid.NewGuid();

        vacationRequest.Reject(rejectedByUserId);

        Assert.Equal(VacationRequestStatus.Rejected, vacationRequest.Status);
        Assert.Equal(rejectedByUserId, vacationRequest.RejectedByUserId);
        Assert.NotNull(vacationRequest.UpdatedAt);
    }

    [Fact]
    public void Approve_ShouldThrowInvalidOperationException_WhenVacationRequestIsNotPending()
    {
        var vacationRequest = new VacationRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 7, 10),
            new DateOnly(2026, 7, 20),
            VacationRequestStatus.Approved);

        Assert.Throws<InvalidOperationException>(() => vacationRequest.Approve(Guid.NewGuid()));
    }

    [Fact]
    public void Reject_ShouldThrowInvalidOperationException_WhenVacationRequestIsNotPending()
    {
        var vacationRequest = new VacationRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 7, 10),
            new DateOnly(2026, 7, 20),
            VacationRequestStatus.Rejected);

        Assert.Throws<InvalidOperationException>(() => vacationRequest.Reject(Guid.NewGuid()));
    }

    [Fact]
    public void Cancel_ShouldChangeStatusToCancelled_WhenVacationRequestIsPending()
    {
        var vacationRequest = new VacationRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 7, 10),
            new DateOnly(2026, 7, 20),
            VacationRequestStatus.Pending);

        var cancelledByUserId = Guid.NewGuid();

        vacationRequest.Cancel(cancelledByUserId);

        Assert.Equal(VacationRequestStatus.Cancelled, vacationRequest.Status);
        Assert.Equal(cancelledByUserId, vacationRequest.CancelledByUserId);
        Assert.NotNull(vacationRequest.UpdatedAt);
    }

    [Fact]
    public void Cancel_ShouldThrowInvalidOperationException_WhenVacationRequestIsApproved()
    {
        var vacationRequest = new VacationRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 7, 10),
            new DateOnly(2026, 7, 20),
            VacationRequestStatus.Approved);

        Assert.Throws<InvalidOperationException>(() => vacationRequest.Cancel(Guid.NewGuid()));
    }
}