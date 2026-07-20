using HospitalVacationManagement.Application.Calendar;
using HospitalVacationManagement.Domain.Vacations;
using HospitalVacationManagement.Tests.Fakes;
using Xunit;

namespace HospitalVacationManagement.Tests.Calendar;

public sealed class ListVacationCalendarHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnApprovedVacationsForSelectedMonth()
    {
        var departmentId = Guid.NewGuid();

        var vacationRequestRepository = new FakeVacationRequestRepository();

        var approvedVacation = new VacationRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            departmentId,
            new DateOnly(2026, 7, 10),
            new DateOnly(2026, 7, 20),
            VacationRequestStatus.Approved);

        var pendingVacation = new VacationRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            departmentId,
            new DateOnly(2026, 7, 15),
            new DateOnly(2026, 7, 25),
            VacationRequestStatus.Pending);

        await vacationRequestRepository.AddAsync(
            approvedVacation,
            CancellationToken.None);

        await vacationRequestRepository.AddAsync(
            pendingVacation,
            CancellationToken.None);

        var handler = new ListVacationCalendarHandler(vacationRequestRepository);

        var request = new ListVacationCalendarRequest(
            departmentId,
            2026,
            7);

        var result = await handler.HandleAsync(request, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(approvedVacation.Id, result.First().VacationRequestId);
    }
}