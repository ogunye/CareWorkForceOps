using CareWorkOps.Domain.Common;
using CareWorkOps.Domain.Staff;
using FluentAssertions;

namespace CareWorkOps.Domain.Tests.Staff;

public sealed class StaffMemberAggregateTests
{
    [Fact]
    public void AddEmploymentRecord_Should_Set_EmploymentRecord()
    {
        var staff = CreateStaff();

        var employmentRecord = EmploymentRecord.Create(
            "Care Assistant",
            EmploymentType.FullTime,
            DateOnly.FromDateTime(DateTime.UtcNow.Date));

        staff.AddEmploymentRecord(employmentRecord);

        staff.EmploymentRecord.Should().NotBeNull();
        staff.EmploymentRecord!.JobTitle.Should().Be("Care Assistant");
        staff.EmploymentRecord.EmploymentType.Should().Be(EmploymentType.FullTime);
    }

    [Fact]
    public void AddEmploymentRecord_Should_Throw_When_Staff_Is_Archived()
    {
        var staff = CreateStaff();
        staff.Archive();

        var employmentRecord = EmploymentRecord.Create(
            "Care Assistant",
            EmploymentType.FullTime,
            DateOnly.FromDateTime(DateTime.UtcNow.Date));

        var action = () => staff.AddEmploymentRecord(employmentRecord);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*Archived staff cannot be modified*");
    }

    [Fact]
    public void AddDbsCheck_Should_Set_DbsCheck()
    {
        var staff = CreateStaff();

        var dbsCheck = DbsCheck.Create(
            "DBS-123456",
            DbsCheckType.Enhanced,
            DateOnly.FromDateTime(DateTime.UtcNow.Date));

        staff.AddDbsCheck(dbsCheck);

        staff.DbsCheck.Should().NotBeNull();
        staff.DbsCheck!.CertificateNumber.Should().Be("DBS-123456");
        staff.DbsCheck.Status.Should().Be(DbsCheckStatus.Clear);
    }

    [Fact]
    public void AddDbsCheck_Should_Throw_When_Staff_Is_Archived()
    {
        var staff = CreateStaff();
        staff.Archive();

        var dbsCheck = DbsCheck.Create(
            "DBS-123456",
            DbsCheckType.Enhanced,
            DateOnly.FromDateTime(DateTime.UtcNow.Date));

        var action = () => staff.AddDbsCheck(dbsCheck);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*Archived staff cannot be modified*");
    }

    [Fact]
    public void AddTrainingRecord_Should_Add_Training_Record()
    {
        var staff = CreateStaff();

        var training = TrainingRecord.Create(
            "Medication Administration",
            TrainingType.Mandatory,
            DateOnly.FromDateTime(DateTime.UtcNow.Date),
            validityMonths: 12);

        staff.AddTrainingRecord(training);

        staff.TrainingRecords.Should().ContainSingle();
        staff.TrainingRecords[0].TrainingName.Should().Be("Medication Administration");
    }

    [Fact]
    public void AddTrainingRecord_Should_Throw_When_Duplicate_TrainingName_Exists()
    {
        var staff = CreateStaff();

        var firstTraining = TrainingRecord.Create(
            "Medication Administration",
            TrainingType.Mandatory,
            DateOnly.FromDateTime(DateTime.UtcNow.Date),
            validityMonths: 12);

        var duplicateTraining = TrainingRecord.Create(
            "Medication Administration",
            TrainingType.Mandatory,
            DateOnly.FromDateTime(DateTime.UtcNow.Date),
            validityMonths: 12);

        staff.AddTrainingRecord(firstTraining);

        var action = () => staff.AddTrainingRecord(duplicateTraining);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*Training record already exists*");
    }

    [Fact]
    public void AddTrainingRecord_Should_Throw_When_Staff_Is_Archived()
    {
        var staff = CreateStaff();
        staff.Archive();

        var training = TrainingRecord.Create(
            "Medication Administration",
            TrainingType.Mandatory,
            DateOnly.FromDateTime(DateTime.UtcNow.Date),
            validityMonths: 12);

        var action = () => staff.AddTrainingRecord(training);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*Archived staff cannot be modified*");
    }

    [Fact]
    public void AddAvailability_Should_Add_Availability()
    {
        var staff = CreateStaff();

        var availability = StaffAvailability.Create(
            AvailabilityDay.Monday,
            AvailabilityShift.Morning,
            AvailabilityStatus.Available);

        staff.AddAvailability(availability);

        staff.Availability.Should().ContainSingle();
        staff.Availability[0].Day.Should().Be(AvailabilityDay.Monday);
        staff.Availability[0].Shift.Should().Be(AvailabilityShift.Morning);
    }

    [Fact]
    public void AddAvailability_Should_Throw_When_Duplicate_Day_And_Shift_Exists()
    {
        var staff = CreateStaff();

        var firstAvailability = StaffAvailability.Create(
            AvailabilityDay.Monday,
            AvailabilityShift.Morning,
            AvailabilityStatus.Available);

        var duplicateAvailability = StaffAvailability.Create(
            AvailabilityDay.Monday,
            AvailabilityShift.Morning,
            AvailabilityStatus.Unavailable,
            "School run.");

        staff.AddAvailability(firstAvailability);

        var action = () => staff.AddAvailability(duplicateAvailability);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*Availability already exists for this day and shift*");
    }

    [Fact]
    public void AddAvailability_Should_Throw_When_Staff_Is_Archived()
    {
        var staff = CreateStaff();
        staff.Archive();

        var availability = StaffAvailability.Create(
            AvailabilityDay.Monday,
            AvailabilityShift.Morning,
            AvailabilityStatus.Available);

        var action = () => staff.AddAvailability(availability);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*Archived staff cannot be modified*");
    }

    [Fact]
    public void AddEmergencyContact_Should_Add_Contact()
    {
        var staff = CreateStaff();

        var contact = EmergencyContact.Create(
            "Sarah Jones",
            EmergencyContactRelationship.Spouse,
            "07700111222",
            "sarah.jones@example.com",
            isPrimary: true);

        staff.AddEmergencyContact(contact);

        staff.EmergencyContacts.Should().ContainSingle();
        staff.EmergencyContacts[0].FullName.Should().Be("Sarah Jones");
        staff.EmergencyContacts[0].IsPrimary.Should().BeTrue();
    }

    [Fact]
    public void AddEmergencyContact_Should_Ensure_Only_One_Primary_Contact()
    {
        var staff = CreateStaff();

        var firstContact = EmergencyContact.Create(
            "Sarah Jones",
            EmergencyContactRelationship.Spouse,
            "07700111222",
            "sarah.jones@example.com",
            isPrimary: true);

        var secondContact = EmergencyContact.Create(
            "Daniel Jones",
            EmergencyContactRelationship.Parent,
            "07700111333",
            "daniel.jones@example.com",
            isPrimary: true);

        staff.AddEmergencyContact(firstContact);
        staff.AddEmergencyContact(secondContact);

        staff.EmergencyContacts.Should().HaveCount(2);
        staff.EmergencyContacts.Count(x => x.IsPrimary).Should().Be(1);
        staff.EmergencyContacts[0].IsPrimary.Should().BeFalse();
        staff.EmergencyContacts[1].IsPrimary.Should().BeTrue();
    }

    [Fact]
    public void AddEmergencyContact_Should_Throw_When_Staff_Is_Archived()
    {
        var staff = CreateStaff();
        staff.Archive();

        var contact = EmergencyContact.Create(
            "Sarah Jones",
            EmergencyContactRelationship.Spouse,
            "07700111222",
            "sarah.jones@example.com",
            isPrimary: true);

        var action = () => staff.AddEmergencyContact(contact);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*Archived staff cannot be modified*");
    }

    [Fact]
    public void StaffMember_Should_Expose_ReadOnly_Collections()
    {
        var staff = CreateStaff();

        staff.TrainingRecords.Should().BeAssignableTo<IReadOnlyList<TrainingRecord>>();
        staff.Availability.Should().BeAssignableTo<IReadOnlyList<StaffAvailability>>();
        staff.EmergencyContacts.Should().BeAssignableTo<IReadOnlyList<EmergencyContact>>();
    }

    private static StaffMember CreateStaff()
    {
        return StaffMember.Create(
            Guid.NewGuid(),
            "Mary",
            "Jones",
            "mary.jones@careworkops.com",
            "07700111222",
            StaffRole.Carer);
    }
}