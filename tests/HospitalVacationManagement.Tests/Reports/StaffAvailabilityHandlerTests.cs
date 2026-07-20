using HospitalVacationManagement.Application.Reports;
using HospitalVacationManagement.Domain.Employees;
using HospitalVacationManagement.Domain.Vacations;
using HospitalVacationManagement.Tests.Fakes;
using HospitalVacationManagement.Domain.Departments;
using Xunit;

namespace HospitalVacationManagement.Tests.Reports;

public sealed class StaffAvailabilityHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCalculateStaffAvailability()
    {
        var departmentId = Guid.NewGuid();

        var departmentRepository = new FakeDepartmentRepository();

        var employeeRepository = new FakeEmployeeRepository();
        var vacationRequestRepository = new FakeVacationRequestRepository();

        var juniorAvailable = new Employee(
            Guid.NewGuid(),
            "Joao Junior",
            departmentId,
            JobRole.Nurse,
            SeniorityLevel.Junior);

        var seniorAvailable = new Employee(
            Guid.NewGuid(),
            "Ana Senior",
            departmentId,
            JobRole.Nurse,
            SeniorityLevel.Senior);

        var juniorOnVacation = new Employee(
            Guid.NewGuid(),
            "Bruno Ferias",
            departmentId,
            JobRole.Nurse,
            SeniorityLevel.Junior);

        employeeRepository.Add(juniorAvailable);
        employeeRepository.Add(seniorAvailable);
        employeeRepository.Add(juniorOnVacation);

        var approvedVacation = new VacationRequest(
            Guid.NewGuid(),
            juniorOnVacation.Id,
            departmentId,
            new DateOnly(2026, 7, 10),
            new DateOnly(2026, 7, 20),
            VacationRequestStatus.Approved);

        await vacationRequestRepository.AddAsync(
            approvedVacation,
            CancellationToken.None);

        var handler = new StaffAvailabilityHandler(
            departmentRepository,
            employeeRepository,
            vacationRequestRepository);

        var request = new StaffAvailabilityRequest(
            departmentId,
            new DateOnly(2026, 7, 15),
            new DateOnly(2026, 7, 16));

        var department = new Department(
            departmentId,
            "Emergency",
            maximumSimultaneousVacations: 2);

        departmentRepository.Add(department);

        var result = await handler.HandleAsync(request, CancellationToken.None);

        Assert.Equal(departmentId, result.DepartmentId);
        Assert.Equal(3, result.TotalEmployees);
        Assert.Equal(1, result.EmployeesOnVacation);
        Assert.Equal(2, result.AvailableEmployees);
        Assert.Equal(1, result.AvailableSeniorEmployees);
        Assert.Equal("Emergency", result.DepartmentName);
    }
}