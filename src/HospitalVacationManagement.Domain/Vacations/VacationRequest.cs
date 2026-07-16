namespace HospitalVacationManagement.Domain.Vacations;

public sealed class VacationRequest
{
    public VacationRequest(Guid id, Guid employeeId, DateOnly startDate, DateOnly endDate, VacationRequestStatus status)
    {
        if (endDate < startDate)
        {
            throw new ArgumentException("A data de término das férias não pode ser anterior à data de início.");
        }

        Id = id;
        EmployeeId = employeeId;
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
    }

    public void Approve()
    {
        if (Status != VacationRequestStatus.Pending)
        {
            throw new InvalidOperationException("Only pending vacation requests can be approved.");
        }

        Status = VacationRequestStatus.Approved;
    }

    public void Reject()
    {
        if (Status != VacationRequestStatus.Pending)
        {
            throw new InvalidOperationException("Only pending vacation requests can be rejected.");
        }

        Status = VacationRequestStatus.Rejected;
    }

    public Guid Id { get; }
    public Guid EmployeeId { get; }
    public DateOnly StartDate { get; }
    public DateOnly EndDate { get; }
    public VacationRequestStatus Status { get; private set; }

    public bool Overlaps(DateOnly startDate, DateOnly endDate)
    {
        return StartDate <= endDate && startDate <= EndDate;
    }
}