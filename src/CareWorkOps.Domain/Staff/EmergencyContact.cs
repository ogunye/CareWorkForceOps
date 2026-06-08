using System.Text.RegularExpressions;
using CareWorkOps.Domain.Common;

namespace CareWorkOps.Domain.Staff;

public sealed class EmergencyContact : ValueObject
{
    private const int FullNameMaxLength = 150;
    private const int PhoneNumberMaxLength = 30;
    private const int EmailMaxLength = 250;

    private static readonly Regex EmailRegex =
        new("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$", RegexOptions.Compiled);

    public string FullName { get; private set; } = string.Empty;

    public EmergencyContactRelationship Relationship { get; private set; }

    public string PhoneNumber { get; private set; } = string.Empty;

    public string? Email { get; private set; }

    public bool IsPrimary { get; private set; }

    private EmergencyContact()
    {
    }

    private EmergencyContact(
        string fullName,
        EmergencyContactRelationship relationship,
        string phoneNumber,
        string? email,
        bool isPrimary)
    {
        SetContactDetails(
            fullName,
            relationship,
            phoneNumber,
            email);

        IsPrimary = isPrimary;
    }

    public static EmergencyContact Create(
        string fullName,
        EmergencyContactRelationship relationship,
        string phoneNumber,
        string? email,
        bool isPrimary)
    {
        return new EmergencyContact(
            fullName,
            relationship,
            phoneNumber,
            email,
            isPrimary);
    }

    public void Update(
        string fullName,
        EmergencyContactRelationship relationship,
        string phoneNumber,
        string? email)
    {
        SetContactDetails(
            fullName,
            relationship,
            phoneNumber,
            email);
    }

    public void MarkPrimary()
    {
        IsPrimary = true;
    }

    public void MarkSecondary()
    {
        IsPrimary = false;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FullName;
        yield return Relationship;
        yield return PhoneNumber;
        yield return Email;
        yield return IsPrimary;
    }

    private void SetContactDetails(
        string fullName,
        EmergencyContactRelationship relationship,
        string phoneNumber,
        string? email)
    {
        FullName = ValidateFullName(fullName);
        Relationship = relationship;
        PhoneNumber = ValidatePhoneNumber(phoneNumber);
        Email = NormalizeOptionalEmail(email);
    }

    private static string ValidateFullName(string fullName)
    {
        return Guard.AgainstNullOrWhiteSpace(
            fullName,
            nameof(FullName),
            FullNameMaxLength);
    }

    private static string ValidatePhoneNumber(string phoneNumber)
    {
        return Guard.AgainstNullOrWhiteSpace(
            phoneNumber,
            nameof(PhoneNumber),
            PhoneNumberMaxLength);
    }

    private static string? NormalizeOptionalEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        email = email.Trim().ToLowerInvariant();

        if (email.Length > EmailMaxLength)
        {
            throw new DomainException($"Email cannot exceed {EmailMaxLength} characters.");
        }

        if (!EmailRegex.IsMatch(email))
        {
            throw new DomainException("Email is invalid.");
        }

        return email;
    }
}