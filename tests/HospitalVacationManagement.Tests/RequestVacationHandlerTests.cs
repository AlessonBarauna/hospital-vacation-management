using HospitalVacationManagement.Application.Vacations;
using HospitalVacationManagement.Domain.Departments;
using HospitalVacationManagement.Domain.Employees;
using HospitalVacationManagement.Domain.Vacations;
using HospitalVacationManagement.Tests.Fakes;
using Xunit;

namespace HospitalVacationManagement.Tests;

public sealed class RequestVacationHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenEmployeeDoesNotExist()
    {
        var employeeRepository = new FakeEmployeeRepository();
        var departmentRepository = new FakeDepartmentRepository();
        var vacationRequestRepository = new FakeVacationRequestRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new RequestVacationHandler(
            employeeRepository,
            departmentRepository,
            vacationRequestRepository,
            new VacationPolicyService(),
            unitOfWork);

        var request = new RequestVacationRequest(
            Guid.NewGuid(),
            new DateOnly(2026, 8, 1),
            new DateOnly(2026, 8, 10));

        var response = await handler.HandleAsync(request, CancellationToken.None);

        Assert.False(response.IsValid);
        Assert.Contains("Employee was not found.", response.Errors);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenEmployeeHasOverlappingRequest()
    {
        var departmentId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();

        var employeeRepository = new FakeEmployeeRepository();
        var departmentRepository = new FakeDepartmentRepository();
        var vacationRequestRepository = new FakeVacationRequestRepository();
        var unitOfWork = new FakeUnitOfWork();

        employeeRepository.Add(new Employee(
            employeeId,
            "Maria Oliveira",
            departmentId,
            JobRole.Nurse,
            SeniorityLevel.Senior));

        vacationRequestRepository.Add(new VacationRequest(
            Guid.NewGuid(),
            employeeId,
            new DateOnly(2026, 8, 1),
            new DateOnly(2026, 8, 10),
            VacationRequestStatus.Pending));

        var handler = new RequestVacationHandler(
            employeeRepository,
            departmentRepository,
            vacationRequestRepository,
            new VacationPolicyService(),
            unitOfWork);

        var request = new RequestVacationRequest(
            employeeId,
            new DateOnly(2026, 8, 5),
            new DateOnly(2026, 8, 15));

        var response = await handler.HandleAsync(request, CancellationToken.None);

        Assert.False(response.IsValid);
        Assert.Contains("Employee already has a pending or approved vacation request in this period.", response.Errors);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateVacationRequest_WhenRequestIsValid()
    {
        var departmentId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var seniorId = Guid.NewGuid();

        var employeeRepository = new FakeEmployeeRepository();
        var departmentRepository = new FakeDepartmentRepository();
        var vacationRequestRepository = new FakeVacationRequestRepository();
        var unitOfWork = new FakeUnitOfWork();

        departmentRepository.Add(new Department(
            departmentId,
            "Emergency",
            maximumSimultaneousVacations: 2));

        employeeRepository.Add(new Employee(
            employeeId,
            "Maria Oliveira",
            departmentId,
            JobRole.Nurse,
            SeniorityLevel.Junior));

        employeeRepository.Add(new Employee(
            seniorId,
            "Ana Silva",
            departmentId,
            JobRole.Nurse,
            SeniorityLevel.Senior));

        var handler = new RequestVacationHandler(
            employeeRepository,
            departmentRepository,
            vacationRequestRepository,
            new VacationPolicyService(),
            unitOfWork);

        var request = new RequestVacationRequest(
            employeeId,
            new DateOnly(2026, 8, 1),
            new DateOnly(2026, 8, 10));

        var response = await handler.HandleAsync(request, CancellationToken.None);

        Assert.True(response.IsValid);
        Assert.NotNull(response.VacationRequestId);
        Assert.Single(vacationRequestRepository.VacationRequests);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }
}