using CareWorkOps.Domain.Common;
using CareWorkOps.Domain.Staff;
using FluentAssertions;

namespace CareWorkOps.Domain.Tests.Staff;

public sealed class StaffAvailabilityTests
{
    [Fact]
    public void Create_Should_Create_StaffAvailability_With_Valid_Data()
    {
        var availability = StaffAvailability.Create(
            AvailabilityDay.Monday,
            AvailabilityShift.Morning,
            AvailabilityStatus.Available);

        availability.Day.Should().Be(AvailabilityDay.Monday);
        availability.Shift.Should().Be(AvailabilityShift.Morning);
        availability.Status.Should().Be(AvailabilityStatus.Available);
        availability.Notes.Should().BeNull();
    }

    [Fact]
    public void Create_Should_Create_StaffAvailability_With_Notes()
    {
        var availability = StaffAvailability.Create(
            AvailabilityDay.Tuesday,
            AvailabilityShift.Night,
            AvailabilityStatus.Unavailable,
            "Childcare commitment.");

        availability.Day.Should().Be(AvailabilityDay.Tuesday);
        availability.Shift.Should().Be(AvailabilityShift.Night);
        availability.Status.Should().Be(AvailabilityStatus.Unavailable);
        availability.Notes.Should().Be("Childcare commitment.");
    }

    [Fact]
    public void MarkAvailable_Should_Set_Status_To_Available_And_Clear_Notes()
    {
        var availability = StaffAvailability.Create(
            AvailabilityDay.Wednesday,
            AvailabilityShift.Afternoon,
            AvailabilityStatus.Unavailable,
            "Previous commitment.");

        availability.MarkAvailable();

        availability.Status.Should().Be(AvailabilityStatus.Available);
        availability.Notes.Should().BeNull();
    }

    [Fact]
    public void MarkUnavailable_Should_Set_Status_To_Unavailable_With_Reason()
    {
        var availability = StaffAvailability.Create(
            AvailabilityDay.Thursday,
            AvailabilityShift.Evening,
            AvailabilityStatus.Available);

        availability.MarkUnavailable("Medical appointment.");

        availability.Status.Should().Be(AvailabilityStatus.Unavailable);
        availability.Notes.Should().Be("Medical appointment.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void MarkUnavailable_Should_Throw_When_Reason_Is_Invalid(string? reason)
    {
        var availability = StaffAvailability.Create(
            AvailabilityDay.Friday,
            AvailabilityShift.Morning,
            AvailabilityStatus.Available);

        var action = () => availability.MarkUnavailable(reason!);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*Notes is required*");
    }

    [Fact]
    public void MarkStandby_Should_Set_Status_To_Standby_With_Optional_Notes()
    {
        var availability = StaffAvailability.Create(
            AvailabilityDay.Saturday,
            AvailabilityShift.Night,
            AvailabilityStatus.Available);

        availability.MarkStandby("Can cover emergency night shift.");

        availability.Status.Should().Be(AvailabilityStatus.Standby);
        availability.Notes.Should().Be("Can cover emergency night shift.");
    }

    [Fact]
    public void ChangeShift_Should_Update_Day_And_Shift()
    {
        var availability = StaffAvailability.Create(
            AvailabilityDay.Sunday,
            AvailabilityShift.Morning,
            AvailabilityStatus.Available);

        availability.ChangeShift(
            AvailabilityDay.Monday,
            AvailabilityShift.Evening);

        availability.Day.Should().Be(AvailabilityDay.Monday);
        availability.Shift.Should().Be(AvailabilityShift.Evening);
    }

    [Fact]
    public void SameSlotAs_Should_Return_True_When_Day_And_Shift_Match()
    {
        var first = StaffAvailability.Create(
            AvailabilityDay.Monday,
            AvailabilityShift.Morning,
            AvailabilityStatus.Available);

        var second = StaffAvailability.Create(
            AvailabilityDay.Monday,
            AvailabilityShift.Morning,
            AvailabilityStatus.Unavailable,
            "School run.");

        first.SameSlotAs(second).Should().BeTrue();
    }

    [Fact]
    public void SameSlotAs_Should_Return_False_When_Day_Or_Shift_Does_Not_Match()
    {
        var first = StaffAvailability.Create(
            AvailabilityDay.Monday,
            AvailabilityShift.Morning,
            AvailabilityStatus.Available);

        var second = StaffAvailability.Create(
            AvailabilityDay.Tuesday,
            AvailabilityShift.Morning,
            AvailabilityStatus.Available);

        first.SameSlotAs(second).Should().BeFalse();
    }

    [Fact]
    public void Create_Should_Normalize_Notes()
    {
        var availability = StaffAvailability.Create(
            AvailabilityDay.Monday,
            AvailabilityShift.Morning,
            AvailabilityStatus.Standby,
            "  Can cover short notice.  ");

        availability.Notes.Should().Be("Can cover short notice.");
    }
}