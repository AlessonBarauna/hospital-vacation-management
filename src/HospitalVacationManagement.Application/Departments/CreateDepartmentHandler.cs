using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Domain.Departments;

namespace HospitalVacationManagement.Application.Departments;

public sealed class CreateDepartmentHandler
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDepartmentHandler(
        IDepartmentRepository departmentRepository,
        IUnitOfWork unitOfWork)
    {
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DepartmentResponse> HandleAsync(
        CreateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var department = new Department(
            Guid.NewGuid(),
            request.Name,
            request.MaximumSimultaneousVacations);

        await _departmentRepository.AddAsync(department, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DepartmentResponse(
            department.Id,
            department.Name,
            department.MaximumSimultaneousVacations);
    }
}