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

        vacationRequest.Approve();

        Assert.Equal(VacationRequestStatus.Approved, vacationRequest.Status);
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

        vacationRequest.Reject();

        Assert.Equal(VacationRequestStatus.Rejected, vacationRequest.Status);
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

        Assert.Throws<InvalidOperationException>(() => vacationRequest.Approve());
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

        Assert.Throws<InvalidOperationException>(() => vacationRequest.Reject());
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

        vacationRequest.Cancel();

        Assert.Equal(VacationRequestStatus.Cancelled, vacationRequest.Status);
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

        Assert.Throws<InvalidOperationException>(() => vacationRequest.Cancel());
    }
}