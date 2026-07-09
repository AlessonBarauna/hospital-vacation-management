using HospitalVacationManagement.Domain.Departments;
using HospitalVacationManagement.Domain.Employees;

namespace HospitalVacationManagement.Domain.Vacations;

public interface IVacationPolicyService
{
    VacationValidationResult Validate(
        Employee employee,
        Department department,
        IReadOnlyCollection<Employee> departmentEmployees,
        IReadOnlyCollection<VacationRequest> approvedVacations,
        DateOnly startDate,
        DateOnly endDate);
}