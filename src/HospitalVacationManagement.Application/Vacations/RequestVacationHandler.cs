using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Domain.Vacations;

namespace HospitalVacationManagement.Application.Vacations;

public sealed class RequestVacationHandler
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IVacationRequestRepository _vacationRequestRepository;
    private readonly IVacationPolicyService _vacationPolicyService;
    private readonly IUnitOfWork _unitOfWork;

    public RequestVacationHandler(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IVacationRequestRepository vacationRequestRepository,
        IVacationPolicyService vacationPolicyService,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _vacationRequestRepository = vacationRequestRepository;
        _vacationPolicyService = vacationPolicyService;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestVacationResponse> HandleAsync(
        RequestVacationRequest request,
         Guid currentUserId,
        CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);

        if (employee is null)
        {
            return new RequestVacationResponse(false, null, ["Employee was not found."]);
        }

        var hasOverlappingRequest = await _vacationRequestRepository.HasOverlappingRequestForEmployeeAsync(
            employee.Id,
            request.StartDate,
            request.EndDate,
            cancellationToken);

        if (hasOverlappingRequest)
        {
            return new RequestVacationResponse(
                false,
                null,
                ["Employee already has a pending or approved vacation request in this period."]);
        }

        var department = await _departmentRepository.GetByIdAsync(employee.DepartmentId, cancellationToken);

        if (department is null)
        {
            return new RequestVacationResponse(false, null, ["Department was not found."]);
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

        if (!validationResult.IsValid)
        {
            return new RequestVacationResponse(false, null, validationResult.Errors);
        }

        var vacationRequest = new VacationRequest(
            Guid.NewGuid(),
            employee.Id,
            employee.DepartmentId,
            request.StartDate,
            request.EndDate,
            VacationRequestStatus.Pending,
            currentUserId);

        await _vacationRequestRepository.AddAsync(vacationRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RequestVacationResponse(true, vacationRequest.Id, []);
    }
}