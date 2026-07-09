namespace HospitalVacationManagement.Domain.Employees;

public sealed class Employee
{
    public Employee(Guid id, string fullName, Guid departmentId, JobRole role, SeniorityLevel seniorityLevel)
    {
        Id = id;
        FullName = fullName;
        DepartmentId = departmentId;
        Role = role;
        SeniorityLevel = seniorityLevel;
    }

    public Guid Id { get; }
    public string FullName { get; }
    public Guid DepartmentId { get; }
    public JobRole Role { get; }
    public SeniorityLevel SeniorityLevel { get; }
}