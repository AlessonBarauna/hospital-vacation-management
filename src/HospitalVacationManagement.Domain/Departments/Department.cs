namespace HospitalVacationManagement.Domain.Departments;

public sealed class Department
{
    public Department(Guid id, string name, int maximumSimultaneousVacations)
    {
        Id = id;
        Name = name;
        MaximumSimultaneousVacations = maximumSimultaneousVacations;
    }

    public Guid Id { get; }
    public string Name { get; }
    public int MaximumSimultaneousVacations { get; }
}