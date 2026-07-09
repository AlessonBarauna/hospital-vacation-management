using HospitalVacationManagement.Domain.Departments;
using HospitalVacationManagement.Domain.Employees;

namespace HospitalVacationManagement.Domain.Vacations;

public sealed class VacationPolicyService : IVacationPolicyService
{
    private static readonly (int Month, int Day)[] CriticalDates =
    [
        (12, 24),
        (12, 25),
        (12, 31),
        (1, 1)
    ];

    public VacationValidationResult Validate(
        Employee employee,
        Department department,
        IReadOnlyCollection<Employee> departmentEmployees,
        IReadOnlyCollection<VacationRequest> approvedVacations,
        DateOnly startDate,
        DateOnly endDate)
    {
        var errors = new List<string>();

        if (endDate < startDate)
        {
            errors.Add("Vacation end date cannot be before start date.");
        }

        if (ContainsCriticalDate(startDate, endDate))
        {
            errors.Add("Vacation request overlaps a critical hospital period.");
        }

        var overlappingApprovedVacations = approvedVacations
            .Where(vacation => vacation.Status == VacationRequestStatus.Approved)
            .Where(vacation => vacation.Overlaps(startDate, endDate))
            .ToList();

        if (overlappingApprovedVacations.Count >= department.MaximumSimultaneousVacations)
        {
            errors.Add("Department simultaneous vacation limit was reached.");
        }

        var employeesOnVacationIds = overlappingApprovedVacations
            .Select(vacation => vacation.EmployeeId)
            .Append(employee.Id)
            .ToHashSet();

        var employeesAvailable = departmentEmployees
            .Where(candidate => !employeesOnVacationIds.Contains(candidate.Id))
            .ToList();

        if (employeesAvailable.Count == 0)
        {
            errors.Add("Department cannot be left without available professionals.");
        }

        if (!employeesAvailable.Any(candidate => candidate.SeniorityLevel == SeniorityLevel.Senior))
        {
            errors.Add("At least one senior professional must remain available.");
        }

        return errors.Count == 0
            ? VacationValidationResult.Success()
            : VacationValidationResult.Failure(errors);
    }

    private static bool ContainsCriticalDate(DateOnly startDate, DateOnly endDate)
    {
        for (var day = startDate; day <= endDate; day = day.AddDays(1))
        {
            if (CriticalDates.Any(critical => critical.Month == day.Month && critical.Day == day.Day))
            {
                return true;
            }
        }

        return false;
    }
}