using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Domain.Employees;

namespace HospitalVacationManagement.Application.Employees;

public sealed class CreateEmployeeHandler
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEmployeeHandler(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<EmployeeResponse?> HandleAsync(
        CreateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var department = await _departmentRepository.GetByIdAsync(
            request.DepartmentId,
            cancellationToken);

        if (department is null)
        {
            return null;
        }

        var employee = new Employee(
            Guid.NewGuid(),
            request.FullName,
            request.DepartmentId,
            request.Role,
            request.SeniorityLevel);

        await _employeeRepository.AddAsync(employee, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new EmployeeResponse(
            employee.Id,
            employee.FullName,
            employee.DepartmentId,
            employee.Role,
            employee.SeniorityLevel);
    }
}