using CareWorkOps.Domain.Common;

namespace CareWorkOps.Domain.Staff;

public sealed class EmploymentRecord : ValueObject
{
    private const int JobTitleMaxLength = 150;

    public string JobTitle { get; private set; } = string.Empty;
    public EmploymentType EmploymentType { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public EmploymentStatus Status { get; private set; }

    private EmploymentRecord()
    {
    }

    private EmploymentRecord(
        string jobTitle,
        EmploymentType employmentType,
        DateOnly startDate)
    {
        JobTitle = ValidateJobTitle(jobTitle);
        StartDate = ValidateStartDate(startDate);
        EmploymentType = employmentType;
        EndDate = null;
        Status = EmploymentStatus.Active;
    }

    public static EmploymentRecord Create(
        string jobTitle,
        EmploymentType employmentType,
        DateOnly startDate)
    {
        return new EmploymentRecord(
            jobTitle,
            employmentType,
            startDate);
    }

    public void Terminate(DateOnly endDate)
    {
        EnsureNotTerminated();
        ValidateEndDate(endDate);

        EndDate = endDate;
        Status = EmploymentStatus.Terminated;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return JobTitle;
        yield return EmploymentType;
        yield return StartDate;
        yield return EndDate;
        yield return Status;
    }

    private static string ValidateJobTitle(string jobTitle)
    {
        return Guard.AgainstNullOrWhiteSpace(
            jobTitle,
            nameof(JobTitle),
            JobTitleMaxLength);
    }

    private static DateOnly ValidateStartDate(DateOnly startDate)
    {
        if (startDate > Today())
        {
            throw new DomainException("Start date cannot be in the future.");
        }

        return startDate;
    }

    private void ValidateEndDate(DateOnly endDate)
    {
        if (endDate < StartDate)
        {
            throw new DomainException("End date cannot be before start date.");
        }
    }

    private void EnsureNotTerminated()
    {
        if (Status == EmploymentStatus.Terminated)
        {
            throw new DomainException("Employment record is already terminated.");
        }
    }

    private static DateOnly Today()
    {
        return DateOnly.FromDateTime(DateTime.UtcNow.Date);
    }
}