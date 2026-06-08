using CareWorkOps.Domain.Common;

namespace CareWorkOps.Domain.Staff;

public sealed class DbsCheck : ValueObject
{
    private const int CertificateNumberMaxLength = 100;
    private const int ReviewNotesMaxLength = 500;

    public string CertificateNumber { get; private set; } = string.Empty;

    public DbsCheckType CheckType { get; private set; }

    public DateOnly IssueDate { get; private set; }

    public DateOnly ReviewDate { get; private set; }

    public DbsCheckStatus Status { get; private set; }

    public string? ReviewNotes { get; private set; }

    private DbsCheck()
    {
    }

    private DbsCheck(
        string certificateNumber,
        DbsCheckType checkType,
        DateOnly issueDate)
    {
        ApplyClearCheck(
            certificateNumber,
            checkType,
            issueDate);
    }

    public static DbsCheck Create(
        string certificateNumber,
        DbsCheckType checkType,
        DateOnly issueDate)
    {
        return new DbsCheck(
            certificateNumber,
            checkType,
            issueDate);
    }

    public void MarkAsUnderReview(string reviewNotes)
    {
        ReviewNotes = ValidateReviewNotes(reviewNotes);
        Status = DbsCheckStatus.UnderReview;
    }

    public void MarkAsNotClear(string reviewNotes)
    {
        ReviewNotes = ValidateReviewNotes(reviewNotes);
        Status = DbsCheckStatus.NotClear;
    }

    public void Renew(
        string certificateNumber,
        DbsCheckType checkType,
        DateOnly issueDate)
    {
        ApplyClearCheck(
            certificateNumber,
            checkType,
            issueDate);
    }

    public bool IsExpired()
    {
        return ReviewDate < Today();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return CertificateNumber;
        yield return CheckType;
        yield return IssueDate;
        yield return ReviewDate;
        yield return Status;
        yield return ReviewNotes;
    }

    private void ApplyClearCheck(
        string certificateNumber,
        DbsCheckType checkType,
        DateOnly issueDate)
    {
        CertificateNumber = ValidateCertificateNumber(certificateNumber);
        IssueDate = ValidateIssueDate(issueDate);
        ReviewDate = IssueDate.AddYears(1);
        CheckType = checkType;
        Status = DbsCheckStatus.Clear;
        ReviewNotes = null;
    }

    private static string ValidateCertificateNumber(string certificateNumber)
    {
        return Guard.AgainstNullOrWhiteSpace(
            certificateNumber,
            nameof(CertificateNumber),
            CertificateNumberMaxLength);
    }

    private static DateOnly ValidateIssueDate(DateOnly issueDate)
    {
        if (issueDate > Today())
        {
            throw new DomainException("DBS issue date cannot be in the future.");
        }

        return issueDate;
    }

    private static string ValidateReviewNotes(string reviewNotes)
    {
        return Guard.AgainstNullOrWhiteSpace(
            reviewNotes,
            nameof(ReviewNotes),
            ReviewNotesMaxLength);
    }

    private static DateOnly Today()
    {
        return DateOnly.FromDateTime(DateTime.UtcNow.Date);
    }
}