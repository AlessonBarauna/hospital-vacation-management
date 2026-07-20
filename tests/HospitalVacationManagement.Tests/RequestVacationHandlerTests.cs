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

        var response = await handler.HandleAsync(request, Guid.NewGuid(), CancellationToken.None);

        Assert.False(response.IsValid);
        Assert.Contains("Employee was not found.", response.Errors);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
public async Task HandleAsync_ShouldReturnError_WhenEmployeeHasOverlappingRequest()
{
    var departmentId = Guid.NewGuid();
    var employeeId = Guid.NewGuid();
    var currentUserId = Guid.NewGuid();

    var employeeRepository = new FakeEmployeeRepository();
    var departmentRepository = new FakeDepartmentRepository();
    var vacationRequestRepository = new FakeVacationRequestRepository();
    var unitOfWork = new FakeUnitOfWork();
    var vacationPolicyService = new VacationPolicyService();

    var department = new Department(
        departmentId,
        "Emergency",
        maximumSimultaneousVacations: 2);

    var employee = new Employee(
        employeeId,
        "Maria Gestora",
        departmentId,
        JobRole.Nurse,
        SeniorityLevel.Junior);

    var seniorEmployee = new Employee(
        Guid.NewGuid(),
        "Ana Senior",
        departmentId,
        JobRole.Nurse,
        SeniorityLevel.Senior);

    departmentRepository.Add(department);
    employeeRepository.Add(employee);
    employeeRepository.Add(seniorEmployee);

    var existingVacationRequest = new VacationRequest(
        Guid.NewGuid(),
        employeeId,
        departmentId,
        new DateOnly(2026, 7, 10),
        new DateOnly(2026, 7, 20),
        VacationRequestStatus.Pending,
        currentUserId);

    await vacationRequestRepository.AddAsync(
        existingVacationRequest,
        CancellationToken.None);

    var handler = new RequestVacationHandler(
        employeeRepository,
        departmentRepository,
        vacationRequestRepository,
        vacationPolicyService,
        unitOfWork);

    var request = new RequestVacationRequest(
        employeeId,
        new DateOnly(2026, 7, 15),
        new DateOnly(2026, 7, 25));

    var result = await handler.HandleAsync(
        request,
        currentUserId,
        CancellationToken.None);

    Assert.False(result.IsValid);
    Assert.Contains(
        "Employee already has a pending or approved vacation request in this period.",
        result.Errors);
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

        var response = await handler.HandleAsync(request, Guid.NewGuid(), CancellationToken.None);

        Assert.True(response.IsValid);
        Assert.NotNull(response.VacationRequestId);
        Assert.Single(vacationRequestRepository.VacationRequests);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task HandleAsync_ShouldSetCreatedByUserId_WhenVacationRequestIsCreated()
    {
        var departmentId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();

        var employeeRepository = new FakeEmployeeRepository();
        var departmentRepository = new FakeDepartmentRepository();
        var vacationRequestRepository = new FakeVacationRequestRepository();
        var unitOfWork = new FakeUnitOfWork();
        var vacationPolicyService = new VacationPolicyService();

        var department = new Department(
            departmentId,
            "Emergency",
            maximumSimultaneousVacations: 2);

        var employee = new Employee(
            employeeId,
            "Maria Gestora",
            departmentId,
            JobRole.Nurse,
            SeniorityLevel.Junior);

        var seniorEmployee = new Employee(
            Guid.NewGuid(),
            "Ana Senior",
            departmentId,
            JobRole.Nurse,
            SeniorityLevel.Senior);

        departmentRepository.Add(department);
        employeeRepository.Add(employee);
        employeeRepository.Add(seniorEmployee);

        var handler = new RequestVacationHandler(
            employeeRepository,
            departmentRepository,
            vacationRequestRepository,
            vacationPolicyService,
            unitOfWork);

        var request = new RequestVacationRequest(
            employeeId,
            new DateOnly(2026, 7, 10),
            new DateOnly(2026, 7, 20));

        var response = await handler.HandleAsync(
            request,
            currentUserId,
            CancellationToken.None);

        var vacationRequest = await vacationRequestRepository.GetByIdAsync(
            response.VacationRequestId!.Value,
            CancellationToken.None);

        Assert.True(response.IsValid);
        Assert.NotNull(vacationRequest);
        Assert.Equal(currentUserId, vacationRequest.CreatedByUserId);
    }
}