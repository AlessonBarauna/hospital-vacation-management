using HospitalVacationManagement.Application.Abstractions;

namespace HospitalVacationManagement.Application.Departments;

public sealed class ListDepartmentsHandler
{
    private readonly IDepartmentRepository _departmentRepository;

    public ListDepartmentsHandler(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<IReadOnlyCollection<DepartmentResponse>> HandleAsync(
        CancellationToken cancellationToken)
    {
        var departments = await _departmentRepository.GetAllAsync(cancellationToken);

        return departments
            .Select(department => new DepartmentResponse(
                department.Id,
                department.Name,
                department.MaximumSimultaneousVacations))
            .ToList();
    }
}