namespace HospitalVacationManagement.Application.Reports;

public sealed record VacationsByDepartmentResponse(
    Guid DepartmentId,
    string DepartmentName,
    int ApprovedVacations,
    int PendingVacations,
    int RejectedVacations,
    int CancelledVacations);