using HospitalVacationManagement.Application.Abstractions;

namespace HospitalVacationManagement.Application.Departments;

public sealed class GetDepartmentByIdHandler
{
    private readonly IDepartmentRepository _departmentRepository;

    public GetDepartmentByIdHandler(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<DepartmentResponse?> HandleAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var department = await _departmentRepository.GetByIdAsync(id, cancellationToken);

        if (department is null)
        {
            return null;
        }

        return new DepartmentResponse(
            department.Id,
            department.Name,
            department.MaximumSimultaneousVacations);
    }
}