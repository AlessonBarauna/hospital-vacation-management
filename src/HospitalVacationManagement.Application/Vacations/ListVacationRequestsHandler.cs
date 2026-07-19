using HospitalVacationManagement.Application.Abstractions;

namespace HospitalVacationManagement.Application.Vacations;

public sealed class ListVacationRequestsHandler
{
    private readonly IVacationRequestRepository _vacationRequestRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public ListVacationRequestsHandler(
        IVacationRequestRepository vacationRequestRepository,
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository)
    {
        _vacationRequestRepository = vacationRequestRepository;
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<PagedResponse<VacationRequestSummaryResponse>> HandleAsync(
    ListVacationRequestsRequest request,
    CancellationToken cancellationToken)
    {

        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;
        IReadOnlyCollection<Guid>? employeeIds = null;

        if (request.DepartmentId.HasValue)
        {
            var departmentEmployees = await _employeeRepository.GetByDepartmentIdAsync(
                request.DepartmentId.Value,
                cancellationToken);

            employeeIds = departmentEmployees
                .Select(employee => employee.Id)
                .ToList();
        }
        var vacationRequests = await _vacationRequestRepository.GetAllAsync(
            request.Status,
            request.EmployeeId,
            employeeIds,
            request.StartDate,
            request.EndDate,
            page,
            pageSize,
            cancellationToken);

        var response = new List<VacationRequestSummaryResponse>();

        foreach (var vacationRequest in vacationRequests)
        {
            var employee = await _employeeRepository.GetByIdAsync(
                vacationRequest.EmployeeId,
                cancellationToken);

            if (employee is null)
            {
                continue;
            }

            var department = await _departmentRepository.GetByIdAsync(
                employee.DepartmentId,
                cancellationToken);

            if (department is null)
            {
                continue;
            }

            response.Add(new VacationRequestSummaryResponse(
                vacationRequest.Id,
                employee.Id,
                employee.FullName,
                department.Id,
                department.Name,
                employee.Role,
                employee.SeniorityLevel,
                vacationRequest.StartDate,
                vacationRequest.EndDate,
                vacationRequest.CreatedAt,
                vacationRequest.UpdatedAt,
                vacationRequest.Status));
        }

        var totalItems = await _vacationRequestRepository.CountAsync(
            request.Status,
            request.EmployeeId,
            employeeIds,
            request.StartDate,
            request.EndDate,
            cancellationToken);

        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        return new PagedResponse<VacationRequestSummaryResponse>(
            response,
            page,
            pageSize,
            totalItems,
            totalPages);
    }
}