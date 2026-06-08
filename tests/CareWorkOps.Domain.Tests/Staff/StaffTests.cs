using CareWorkOps.Domain.Common;
using CareWorkOps.Domain.Staff;
using FluentAssertions;

namespace CareWorkOps.Domain.Tests.Staff;

public sealed class StaffTests
{
    [Fact]
    public void Create_Should_Create_Staff_With_Valid_Data()
    {
        var tenantId = Guid.NewGuid();

        var staff = StaffMember.Create(
            tenantId,
            "Mary",
            "Jones",
            "mary.jones@careworkops.com",
            "07700111222",
            StaffRole.Carer);

        staff.Id.Value.Should().NotBeEmpty();
        staff.TenantId.Should().Be(tenantId);
        staff.FirstName.Should().Be("Mary");
        staff.LastName.Should().Be("Jones");
        staff.Email.Should().Be("mary.jones@careworkops.com");
        staff.PhoneNumber.Should().Be("07700111222");
        staff.Role.Should().Be(StaffRole.Carer);
        staff.Status.Should().Be(StaffStatus.Active);
        staff.IsDeleted.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_Should_Throw_When_FirstName_Is_Invalid(string? firstName)
    {
        var action = () => StaffMember.Create(
            Guid.NewGuid(),
            firstName!,
            "Jones",
            "mary.jones@careworkops.com",
            "07700111222",
            StaffRole.Carer);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*FirstName is required*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_Should_Throw_When_LastName_Is_Invalid(string? lastName)
    {
        var action = () => StaffMember.Create(
            Guid.NewGuid(),
            "Mary",
            lastName!,
            "mary.jones@careworkops.com",
            "07700111222",
            StaffRole.Carer);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*LastName is required*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-email")]
    [InlineData("mary@")]
    public void Create_Should_Throw_When_Email_Is_Invalid(string? email)
    {
        var action = () => StaffMember.Create(
            Guid.NewGuid(),
            "Mary",
            "Jones",
            email!,
            "07700111222",
            StaffRole.Carer);

        action.Should()
            .Throw<DomainException>();
    }

    [Fact]
    public void Create_Should_Throw_When_TenantId_Is_Empty()
    {
        var action = () => StaffMember.Create(
            Guid.Empty,
            "Mary",
            "Jones",
            "mary.jones@careworkops.com",
            "07700111222",
            StaffRole.Carer);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*TenantId cannot be empty*");
    }

    [Fact]
    public void Deactivate_Should_Set_Status_To_Inactive()
    {
        var staff = CreateStaff();

        staff.Deactivate();

        staff.Status.Should().Be(StaffStatus.Inactive);
    }

    [Fact]
    public void Reactivate_Should_Set_Status_To_Active()
    {
        var staff = CreateStaff();
        staff.Deactivate();

        staff.Reactivate();

        staff.Status.Should().Be(StaffStatus.Active);
    }

    [Fact]
    public void Archive_Should_Set_Status_To_Archived()
    {
        var staff = CreateStaff();

        staff.Archive();

        staff.Status.Should().Be(StaffStatus.Archived);
    }

    [Fact]
    public void UpdateProfile_Should_Update_Basic_Details()
    {
        var staff = CreateStaff();

        staff.UpdateProfile(
            "Sarah",
            "Johnson",
            "sarah.johnson@careworkops.com",
            "07700999888",
            StaffRole.CareCoordinator);

        staff.FirstName.Should().Be("Sarah");
        staff.LastName.Should().Be("Johnson");
        staff.Email.Should().Be("sarah.johnson@careworkops.com");
        staff.PhoneNumber.Should().Be("07700999888");
        staff.Role.Should().Be(StaffRole.CareCoordinator);
    }

    [Fact]
    public void Archived_Staff_Should_Not_Be_Reactivated()
    {
        var staff = CreateStaff();
        staff.Archive();

        var action = staff.Reactivate;

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*Archived staff cannot be modified*");
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