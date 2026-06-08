using CareWorkOps.Domain.Common;

namespace CareWorkOps.Domain.Staff;

public sealed class TrainingRecord : ValueObject
{
    private const int TrainingNameMaxLength = 150;
    private const int ProviderMaxLength = 150;
    private const int CertificateReferenceMaxLength = 100;

    public string TrainingName { get; private set; } = string.Empty;

    public TrainingType TrainingType { get; private set; }

    public DateOnly CompletedDate { get; private set; }

    public DateOnly ExpiryDate { get; private set; }

    public TrainingStatus Status { get; private set; }

    public string? Provider { get; private set; }

    public string? CertificateReference { get; private set; }

    private TrainingRecord()
    {
    }

    private TrainingRecord(
        string trainingName,
        TrainingType trainingType,
        DateOnly completedDate,
        int validityMonths,
        string? provider,
        string? certificateReference)
    {
        ApplyValidTraining(
            trainingName,
            trainingType,
            completedDate,
            validityMonths,
            provider,
            certificateReference);
    }

    public static TrainingRecord Create(
        string trainingName,
        TrainingType trainingType,
        DateOnly completedDate,
        int validityMonths,
        string? provider = null,
        string? certificateReference = null)
    {
        return new TrainingRecord(
            trainingName,
            trainingType,
            completedDate,
            validityMonths,
            provider,
            certificateReference);
    }

    public void MarkAsExpired()
    {
        Status = TrainingStatus.Expired;
    }

    public void Renew(
        DateOnly completedDate,
        int validityMonths,
        string? provider = null,
        string? certificateReference = null)
    {
        ApplyValidTraining(
            TrainingName,
            TrainingType,
            completedDate,
            validityMonths,
            provider,
            certificateReference);
    }

    public bool IsExpired()
    {
        return ExpiryDate < Today();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return TrainingName;
        yield return TrainingType;
        yield return CompletedDate;
        yield return ExpiryDate;
        yield return Status;
        yield return Provider;
        yield return CertificateReference;
    }

    private void ApplyValidTraining(
        string trainingName,
        TrainingType trainingType,
        DateOnly completedDate,
        int validityMonths,
        string? provider,
        string? certificateReference)
    {
        TrainingName = ValidateTrainingName(trainingName);
        TrainingType = trainingType;
        CompletedDate = ValidateCompletedDate(completedDate);

        EnsureValidityMonthsIsValid(validityMonths);

        ExpiryDate = CalculateExpiryDate(CompletedDate, validityMonths);
        Provider = NormalizeOptionalText(provider, ProviderMaxLength);
        CertificateReference = NormalizeOptionalText(
            certificateReference,
            CertificateReferenceMaxLength);

        Status = TrainingStatus.Valid;
    }

    private static string ValidateTrainingName(string trainingName)
    {
        return Guard.AgainstNullOrWhiteSpace(
            trainingName,
            nameof(TrainingName),
            TrainingNameMaxLength);
    }

    private static DateOnly ValidateCompletedDate(DateOnly completedDate)
    {
        if (completedDate > Today())
        {
            throw new DomainException("Training completion date cannot be in the future.");
        }

        return completedDate;
    }

    private static void EnsureValidityMonthsIsValid(int validityMonths)
    {
        if (validityMonths <= 0)
        {
            throw new DomainException("Training validity months must be greater than zero.");
        }
    }

    private static DateOnly CalculateExpiryDate(
        DateOnly completedDate,
        int validityMonths)
    {
        return completedDate.AddMonths(validityMonths);
    }

    private static string? NormalizeOptionalText(
        string? value,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        value = value.Trim();

        if (value.Length > maxLength)
        {
            throw new DomainException($"Value cannot exceed {maxLength} characters.");
        }

        return value;
    }

    private static DateOnly Today()
    {
        return DateOnly.FromDateTime(DateTime.UtcNow.Date);
    }
}