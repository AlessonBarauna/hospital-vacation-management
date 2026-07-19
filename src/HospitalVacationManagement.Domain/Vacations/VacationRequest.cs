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
        CreatedAt = DateTime.UtcNow;
    }

    public void Approve()
    {
        if (Status != VacationRequestStatus.Pending)
        {
            throw new InvalidOperationException("Only pending vacation requests can be approved.");
        }

        Status = VacationRequestStatus.Approved;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        if (Status != VacationRequestStatus.Pending)
        {
            throw new InvalidOperationException("Only pending vacation requests can be rejected.");
        }

        Status = VacationRequestStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;

    }

    public void Cancel()
    {
        if (Status == VacationRequestStatus.Cancelled)
        {
            throw new InvalidOperationException("Vacation request is already cancelled.");
        }

        if (Status == VacationRequestStatus.Approved)
        {
            throw new InvalidOperationException("Approved vacation requests cannot be cancelled.");
        }

        Status = VacationRequestStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public Guid EmployeeId { get; }
    public DateOnly StartDate { get; }
    public DateOnly EndDate { get; }
    public VacationRequestStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public bool Overlaps(DateOnly startDate, DateOnly endDate)
    {
        return StartDate <= endDate && startDate <= EndDate;
    }
}