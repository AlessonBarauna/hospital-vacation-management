using HospitalVacationManagement.Domain.Departments;
using HospitalVacationManagement.Domain.Employees;
using HospitalVacationManagement.Domain.Vacations;

namespace HospitalVacationManagement.Infrastructure.Persistence;

public sealed class InMemoryDatabase
{
    public Guid EmergencyDepartmentId { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    public Guid KarolId { get; } = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    public Guid AnaId { get; } = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    public IReadOnlyCollection<Department> Departments { get; }

    public IReadOnlyCollection<Employee> Employees { get; }

    public IReadOnlyCollection<VacationRequest> VacationRequests { get; }

    public InMemoryDatabase()
    {
        Departments =
        [
            new Department(EmergencyDepartmentId, "Emergency", maximumSimultaneousVacations: 1)
        ];

        Employees =
        [
            new Employee(KarolId, "Karol Barauna", EmergencyDepartmentId, JobRole.Nurse, SeniorityLevel.Junior),
            new Employee(AnaId, "Ana Silva", EmergencyDepartmentId, JobRole.Nurse, SeniorityLevel.Senior)
        ];

        VacationRequests = [];
    }
}