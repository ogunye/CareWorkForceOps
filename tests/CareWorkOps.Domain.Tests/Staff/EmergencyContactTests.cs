using CareWorkOps.Domain.Common;
using CareWorkOps.Domain.Staff;
using FluentAssertions;

namespace CareWorkOps.Domain.Tests.Staff;

public sealed class EmergencyContactTests
{
    [Fact]
    public void Create_Should_Create_EmergencyContact_With_Valid_Data()
    {
        var contact = EmergencyContact.Create(
            "Sarah Jones",
            EmergencyContactRelationship.Spouse,
            "07700111222",
            "sarah.jones@example.com",
            isPrimary: true);

        contact.FullName.Should().Be("Sarah Jones");
        contact.Relationship.Should().Be(EmergencyContactRelationship.Spouse);
        contact.PhoneNumber.Should().Be("07700111222");
        contact.Email.Should().Be("sarah.jones@example.com");
        contact.IsPrimary.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Create_EmergencyContact_Without_Email()
    {
        var contact = EmergencyContact.Create(
            "Daniel Jones",
            EmergencyContactRelationship.Parent,
            "07700111333",
            email: null,
            isPrimary: false);

        contact.FullName.Should().Be("Daniel Jones");
        contact.Email.Should().BeNull();
        contact.IsPrimary.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_Should_Throw_When_FullName_Is_Invalid(string? fullName)
    {
        var action = () => EmergencyContact.Create(
            fullName!,
            EmergencyContactRelationship.Spouse,
            "07700111222",
            "sarah.jones@example.com",
            isPrimary: true);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*FullName is required*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_Should_Throw_When_PhoneNumber_Is_Invalid(string? phoneNumber)
    {
        var action = () => EmergencyContact.Create(
            "Sarah Jones",
            EmergencyContactRelationship.Spouse,
            phoneNumber!,
            "sarah.jones@example.com",
            isPrimary: true);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*PhoneNumber is required*");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("sarah@")]
    [InlineData("@example.com")]
    public void Create_Should_Throw_When_Email_Is_Invalid(string email)
    {
        var action = () => EmergencyContact.Create(
            "Sarah Jones",
            EmergencyContactRelationship.Spouse,
            "07700111222",
            email,
            isPrimary: true);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*Email is invalid*");
    }

    [Fact]
    public void Update_Should_Update_EmergencyContact_Details()
    {
        var contact = CreateEmergencyContact();

        contact.Update(
            "Grace Smith",
            EmergencyContactRelationship.Sibling,
            "07700999888",
            "grace.smith@example.com");

        contact.FullName.Should().Be("Grace Smith");
        contact.Relationship.Should().Be(EmergencyContactRelationship.Sibling);
        contact.PhoneNumber.Should().Be("07700999888");
        contact.Email.Should().Be("grace.smith@example.com");
    }

    [Fact]
    public void MarkPrimary_Should_Set_IsPrimary_To_True()
    {
        var contact = EmergencyContact.Create(
            "Sarah Jones",
            EmergencyContactRelationship.Spouse,
            "07700111222",
            email: null,
            isPrimary: false);

        contact.MarkPrimary();

        contact.IsPrimary.Should().BeTrue();
    }

    [Fact]
    public void MarkSecondary_Should_Set_IsPrimary_To_False()
    {
        var contact = EmergencyContact.Create(
            "Sarah Jones",
            EmergencyContactRelationship.Spouse,
            "07700111222",
            email: null,
            isPrimary: true);

        contact.MarkSecondary();

        contact.IsPrimary.Should().BeFalse();
    }

    [Fact]
    public void Create_Should_Normalize_Text_Fields()
    {
        var contact = EmergencyContact.Create(
            "  Sarah Jones  ",
            EmergencyContactRelationship.Spouse,
            "  07700111222  ",
            "  SARAH.JONES@EXAMPLE.COM  ",
            isPrimary: true);

        contact.FullName.Should().Be("Sarah Jones");
        contact.PhoneNumber.Should().Be("07700111222");
        contact.Email.Should().Be("sarah.jones@example.com");
    }

    private static EmergencyContact CreateEmergencyContact()
    {
        return EmergencyContact.Create(
            "Sarah Jones",
            EmergencyContactRelationship.Spouse,
            "07700111222",
            "sarah.jones@example.com",
            isPrimary: true);
    }
}