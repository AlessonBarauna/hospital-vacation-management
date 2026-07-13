using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Domain.Vacations;

namespace HospitalVacationManagement.Application.Vacations;

public sealed class ValidateVacationRequestHandler
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IVacationRequestRepository _vacationRequestRepository;
    private readonly IVacationPolicyService _vacationPolicyService;

    public ValidateVacationRequestHandler(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IVacationRequestRepository vacationRequestRepository,
        IVacationPolicyService vacationPolicyService)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _vacationRequestRepository = vacationRequestRepository;
        _vacationPolicyService = vacationPolicyService;
    }

    public async Task<ValidateVacationResponse> HandleAsync(
        ValidateVacationRequest request,
        CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);

        if (employee is null)
        {
            return new ValidateVacationResponse(false, ["Employee was not found."]);
        }

        var department = await _departmentRepository.GetByIdAsync(employee.DepartmentId, cancellationToken);

        if (department is null)
        {
            return new ValidateVacationResponse(false, ["Department was not found."]);
        }

        var departmentEmployees = await _employeeRepository.GetByDepartmentIdAsync(
            department.Id,
            cancellationToken);

        var approvedVacations = await _vacationRequestRepository.GetApprovedByDepartmentIdAsync(
            department.Id,
            request.StartDate,
            request.EndDate,
            cancellationToken);

        var validationResult = _vacationPolicyService.Validate(
            employee,
            department,
            departmentEmployees,
            approvedVacations,
            request.StartDate,
            request.EndDate);

        return new ValidateVacationResponse(validationResult.IsValid, validationResult.Errors);
    }
}