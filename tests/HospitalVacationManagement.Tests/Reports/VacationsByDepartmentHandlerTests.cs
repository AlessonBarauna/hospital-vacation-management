using HospitalVacationManagement.Application.Reports;
using HospitalVacationManagement.Domain.Departments;
using HospitalVacationManagement.Domain.Vacations;
using HospitalVacationManagement.Tests.Fakes;
using Xunit;

namespace HospitalVacationManagement.Tests.Reports;

public sealed class VacationsByDepartmentHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCountVacationsByDepartmentAndStatus()
    {
        var departmentId = Guid.NewGuid();

        var departmentRepository = new FakeDepartmentRepository();
        var vacationRequestRepository = new FakeVacationRequestRepository();

        var department = new Department(
            departmentId,
            "Emergency",
            maximumSimultaneousVacations: 2);

        departmentRepository.Add(department);

        var approvedVacation = new VacationRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            departmentId,
            new DateOnly(2026, 7, 1),
            new DateOnly(2026, 7, 10),
            VacationRequestStatus.Approved);

        var pendingVacation = new VacationRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            departmentId,
            new DateOnly(2026, 7, 11),
            new DateOnly(2026, 7, 20),
            VacationRequestStatus.Pending);

        var rejectedVacation = new VacationRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            departmentId,
            new DateOnly(2026, 7, 21),
            new DateOnly(2026, 7, 25),
            VacationRequestStatus.Rejected);

        var cancelledVacation = new VacationRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            departmentId,
            new DateOnly(2026, 7, 26),
            new DateOnly(2026, 7, 30),
            VacationRequestStatus.Cancelled);

        await vacationRequestRepository.AddAsync(approvedVacation, CancellationToken.None);
        await vacationRequestRepository.AddAsync(pendingVacation, CancellationToken.None);
        await vacationRequestRepository.AddAsync(rejectedVacation, CancellationToken.None);
        await vacationRequestRepository.AddAsync(cancelledVacation, CancellationToken.None);

        var handler = new VacationsByDepartmentHandler(
            departmentRepository,
            vacationRequestRepository);

        var request = new VacationsByDepartmentRequest(2026, 7);

        var result = await handler.HandleAsync(request, CancellationToken.None);

        var report = Assert.Single(result);

        Assert.Equal(departmentId, report.DepartmentId);
        Assert.Equal("Emergency", report.DepartmentName);
        Assert.Equal(1, report.ApprovedVacations);
        Assert.Equal(1, report.PendingVacations);
        Assert.Equal(1, report.RejectedVacations);
        Assert.Equal(1, report.CancelledVacations);
    }
}