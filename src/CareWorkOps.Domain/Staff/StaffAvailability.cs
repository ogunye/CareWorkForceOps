using CareWorkOps.Domain.Common;

namespace CareWorkOps.Domain.Staff;

public sealed class StaffAvailability : ValueObject
{
    private const int NotesMaxLength = 500;

    public AvailabilityDay Day { get; private set; }

    public AvailabilityShift Shift { get; private set; }

    public AvailabilityStatus Status { get; private set; }

    public string? Notes { get; private set; }

    private StaffAvailability()
    {
    }

    private StaffAvailability(
        AvailabilityDay day,
        AvailabilityShift shift,
        AvailabilityStatus status,
        string? notes)
    {
        SetSlot(day, shift);
        SetStatus(status, notes);
    }

    public static StaffAvailability Create(
        AvailabilityDay day,
        AvailabilityShift shift,
        AvailabilityStatus status,
        string? notes = null)
    {
        return new StaffAvailability(
            day,
            shift,
            status,
            notes);
    }

    public void MarkAvailable()
    {
        Status = AvailabilityStatus.Available;
        Notes = null;
    }

    public void MarkUnavailable(string reason)
    {
        Status = AvailabilityStatus.Unavailable;
        Notes = ValidateRequiredNotes(reason);
    }

    public void MarkStandby(string? notes = null)
    {
        Status = AvailabilityStatus.Standby;
        Notes = NormalizeOptionalNotes(notes);
    }

    public void ChangeShift(
        AvailabilityDay day,
        AvailabilityShift shift)
    {
        SetSlot(day, shift);
    }

    public bool SameSlotAs(StaffAvailability other)
    {
        return Day == other.Day && Shift == other.Shift;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Day;
        yield return Shift;
        yield return Status;
        yield return Notes;
    }

    private void SetSlot(
        AvailabilityDay day,
        AvailabilityShift shift)
    {
        Day = day;
        Shift = shift;
    }

    private void SetStatus(
        AvailabilityStatus status,
        string? notes)
    {
        Status = status;
        Notes = status switch
        {
            AvailabilityStatus.Available => null,
            AvailabilityStatus.Unavailable => ValidateRequiredNotes(notes!),
            AvailabilityStatus.Standby => NormalizeOptionalNotes(notes),
            _ => throw new DomainException("Invalid availability status.")
        };
    }

    private static string ValidateRequiredNotes(string notes)
    {
        return Guard.AgainstNullOrWhiteSpace(
            notes,
            nameof(Notes),
            NotesMaxLength);
    }

    private static string? NormalizeOptionalNotes(string? notes)
    {
        if (string.IsNullOrWhiteSpace(notes))
        {
            return null;
        }

        notes = notes.Trim();

        if (notes.Length > NotesMaxLength)
        {
            throw new DomainException($"Notes cannot exceed {NotesMaxLength} characters.");
        }

        return notes;
    }
}