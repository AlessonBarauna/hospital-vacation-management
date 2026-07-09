using HospitalVacationManagement.Domain.Departments;
using HospitalVacationManagement.Domain.Employees;
using HospitalVacationManagement.Domain.Vacations;
using Xunit;

namespace HospitalVacationManagement.Tests;

public sealed class VacationPolicyServiceTests
{
    [Fact]
    public void Validate_ShouldRejectVacation_WhenDepartmentWouldHaveNoAvailableProfessionals()
    {
        var departmentId = Guid.NewGuid();

        var employee = new Employee(
            Guid.NewGuid(),
            "Karol Barauna",
            departmentId,
            JobRole.Nurse,
            SeniorityLevel.Senior);


        var department = new Department(
            departmentId,
            "Emergency",
            maximumSimultaneousVacations: 1);

        var service = new VacationPolicyService();

        var result = service.Validate(
            employee,
            department,
            [employee],
            [],
            new DateOnly(2026, 7, 10),
            new DateOnly(2026, 7, 20));

        Assert.False(result.IsValid);
        Assert.Contains("Department cannot be left without available professionals.", result.Errors);
    }

    [Fact]
    public void Validate_ShouldRejectVacation_WhenVacationOverlapsCriticalPeriod()
    {
        var departmentId = Guid.NewGuid();

        var employee = new Employee(
            Guid.NewGuid(),
            "Karol Barauna",
            departmentId,
            JobRole.Nurse,
            SeniorityLevel.Junior);

        var seniorEmployee = new Employee(
            Guid.NewGuid(),
            "Ana Silva",
            departmentId,
            JobRole.Nurse,
            SeniorityLevel.Senior);

        var department = new Department(
            departmentId,
            "Emergency",
            maximumSimultaneousVacations: 2);

        var service = new VacationPolicyService();

        var result = service.Validate(
            employee,
            department,
            [employee, seniorEmployee],
            [],
            new DateOnly(2026, 12, 24),
            new DateOnly(2026, 12, 26));

        Assert.False(result.IsValid);
        Assert.Contains("Vacation request overlaps a critical hospital period.", result.Errors);
    }

    [Fact]
    public void Validate_ShouldRejectVacation_WhenSimultaneousVacationLimitWasReached()
    {
        var departmentId = Guid.NewGuid();

        var employee = new Employee(
            Guid.NewGuid(),
            "Karol Barauna",
            departmentId,
            JobRole.Nurse,
            SeniorityLevel.Junior);

        var seniorEmployee = new Employee(
            Guid.NewGuid(),
            "Ana Silva",
            departmentId,
            JobRole.Nurse,
            SeniorityLevel.Senior);

        var employeeAlreadyOnVacation = new Employee(
            Guid.NewGuid(),
            "Bruno Costa",
            departmentId,
            JobRole.Technician,
            SeniorityLevel.Junior);

        var department = new Department(
            departmentId,
            "Emergency",
            maximumSimultaneousVacations: 1);

        var approvedVacation = new VacationRequest(
            Guid.NewGuid(),
            employeeAlreadyOnVacation.Id,
            new DateOnly(2026, 7, 10),
            new DateOnly(2026, 7, 20),
            VacationRequestStatus.Approved);

        var service = new VacationPolicyService();

        var result = service.Validate(
            employee,
            department,
            [employee, seniorEmployee, employeeAlreadyOnVacation],
            [approvedVacation],
            new DateOnly(2026, 7, 15),
            new DateOnly(2026, 7, 25));

        Assert.False(result.IsValid);
        Assert.Contains("Department simultaneous vacation limit was reached.", result.Errors);
    }

    [Fact]
    public void Validate_ShouldRejectVacation_WhenNoSeniorProfessionalWouldRemainAvailable()
    {
        var departmentId = Guid.NewGuid();

        var seniorEmployee = new Employee(
            Guid.NewGuid(),
            "Ana Silva",
            departmentId,
            JobRole.Nurse,
            SeniorityLevel.Senior);

        var juniorEmployee = new Employee(
            Guid.NewGuid(),
            "Bruno Costa",
            departmentId,
            JobRole.Technician,
            SeniorityLevel.Junior);

        var department = new Department(
            departmentId,
            "Emergency",
            maximumSimultaneousVacations: 2);

        var service = new VacationPolicyService();

        var result = service.Validate(
            seniorEmployee,
            department,
            [seniorEmployee, juniorEmployee],
            [],
            new DateOnly(2026, 8, 1),
            new DateOnly(2026, 8, 10));

        Assert.False(result.IsValid);
        Assert.Contains("At least one senior professional must remain available.", result.Errors);
    }
}