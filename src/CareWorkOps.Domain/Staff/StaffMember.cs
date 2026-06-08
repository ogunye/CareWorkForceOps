using System.Text.RegularExpressions;
using CareWorkOps.Domain.Common;

namespace CareWorkOps.Domain.Staff;

public sealed class StaffMember : AuditableEntity<StaffId>
{
    private const int NameMaxLength = 100;
    private const int EmailMaxLength = 250;
    private const int PhoneMaxLength = 30;

    private static readonly Regex EmailRegex =
        new("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$", RegexOptions.Compiled);

    private readonly List<TrainingRecord> _trainingRecords = [];
    private readonly List<StaffAvailability> _availability = [];
    private readonly List<EmergencyContact> _emergencyContacts = [];

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string PhoneNumber { get; private set; } = string.Empty;

    public StaffRole Role { get; private set; }

    public StaffStatus Status { get; private set; }

    public EmploymentRecord? EmploymentRecord { get; private set; }

    public DbsCheck? DbsCheck { get; private set; }

    public IReadOnlyList<TrainingRecord> TrainingRecords => _trainingRecords.AsReadOnly();

    public IReadOnlyList<StaffAvailability> Availability => _availability.AsReadOnly();

    public IReadOnlyList<EmergencyContact> EmergencyContacts => _emergencyContacts.AsReadOnly();

    private StaffMember()
    {
    }

    private StaffMember(
        StaffId id,
        Guid tenantId,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        StaffRole role)
        : base(id, tenantId)
    {
        Guard.AgainstEmptyGuid(tenantId, nameof(TenantId));

        SetProfile(
            firstName,
            lastName,
            email,
            phoneNumber,
            role);

        Status = StaffStatus.Active;
    }

    public static StaffMember Create(
        Guid tenantId,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        StaffRole role)
    {
        return new StaffMember(
            StaffId.New(),
            tenantId,
            firstName,
            lastName,
            email,
            phoneNumber,
            role);
    }

    public void UpdateProfile(
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        StaffRole role)
    {
        EnsureCanBeModified();

        SetProfile(
            firstName,
            lastName,
            email,
            phoneNumber,
            role);
    }

    public void AddEmploymentRecord(EmploymentRecord employmentRecord)
    {
        EnsureCanBeModified();

        EmploymentRecord = employmentRecord
            ?? throw new DomainException("EmploymentRecord is required.");
    }

    public void AddDbsCheck(DbsCheck dbsCheck)
    {
        EnsureCanBeModified();

        DbsCheck = dbsCheck
            ?? throw new DomainException("DbsCheck is required.");
    }

    public void AddTrainingRecord(TrainingRecord trainingRecord)
    {
        EnsureCanBeModified();
        EnsureTrainingRecordIsValid(trainingRecord);

        _trainingRecords.Add(trainingRecord);
    }

    public void AddAvailability(StaffAvailability availability)
    {
        EnsureCanBeModified();
        EnsureAvailabilityIsValid(availability);

        _availability.Add(availability);
    }

    public void AddEmergencyContact(EmergencyContact emergencyContact)
    {
        EnsureCanBeModified();

        if (emergencyContact is null)
        {
            throw new DomainException("EmergencyContact is required.");
        }

        EnsureSinglePrimaryEmergencyContact(emergencyContact);

        _emergencyContacts.Add(emergencyContact);
    }

    public void Deactivate()
    {
        EnsureCanBeModified();

        Status = StaffStatus.Inactive;
    }

    public void Reactivate()
    {
        EnsureCanBeModified();

        Status = StaffStatus.Active;
    }

    public void Archive()
    {
        Status = StaffStatus.Archived;
    }

    private void SetProfile(
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        StaffRole role)
    {
        FirstName = Guard.AgainstNullOrWhiteSpace(
            firstName,
            nameof(FirstName),
            NameMaxLength);

        LastName = Guard.AgainstNullOrWhiteSpace(
            lastName,
            nameof(LastName),
            NameMaxLength);

        Email = ValidateEmail(email);

        PhoneNumber = Guard.AgainstNullOrWhiteSpace(
            phoneNumber,
            nameof(PhoneNumber),
            PhoneMaxLength);

        Role = role;
    }

    private static string ValidateEmail(string? email)
    {
        email = Guard.AgainstNullOrWhiteSpace(
                email,
                nameof(Email),
                EmailMaxLength)
            .Trim()
            .ToLowerInvariant();

        if (!EmailRegex.IsMatch(email))
        {
            throw new DomainException("Email is invalid.");
        }

        return email;
    }

    private void EnsureTrainingRecordIsValid(TrainingRecord trainingRecord)
    {
        if (trainingRecord is null)
        {
            throw new DomainException("TrainingRecord is required.");
        }

        var alreadyExists = _trainingRecords.Any(existingRecord =>
            string.Equals(
                existingRecord.TrainingName,
                trainingRecord.TrainingName,
                StringComparison.OrdinalIgnoreCase));

        if (alreadyExists)
        {
            throw new DomainException("Training record already exists.");
        }
    }

    private void EnsureAvailabilityIsValid(StaffAvailability availability)
    {
        if (availability is null)
        {
            throw new DomainException("Availability is required.");
        }

        var alreadyExists = _availability.Any(existingAvailability =>
            existingAvailability.SameSlotAs(availability));

        if (alreadyExists)
        {
            throw new DomainException("Availability already exists for this day and shift.");
        }
    }

    private void EnsureSinglePrimaryEmergencyContact(EmergencyContact emergencyContact)
    {
        if (!emergencyContact.IsPrimary)
        {
            return;
        }

        foreach (var existingPrimaryContact in _emergencyContacts.Where(x => x.IsPrimary))
        {
            existingPrimaryContact.MarkSecondary();
        }
    }

    private void EnsureCanBeModified()
    {
        if (Status == StaffStatus.Archived)
        {
            throw new DomainException("Archived staff cannot be modified.");
        }
    }
}