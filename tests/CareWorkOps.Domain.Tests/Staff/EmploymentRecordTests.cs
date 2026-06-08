using CareWorkOps.Domain.Common;
using CareWorkOps.Domain.Staff;
using FluentAssertions;

namespace CareWorkOps.Domain.Tests.Staff;

public sealed class EmploymentRecordTests
{
    [Fact]
    public void Create_Should_Create_Employment_Record_With_Valid_Data()
    {
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        var record = EmploymentRecord.Create(
            "Care Assistant",
            EmploymentType.FullTime,
            startDate);

        record.JobTitle.Should().Be("Care Assistant");
        record.EmploymentType.Should().Be(EmploymentType.FullTime);
        record.StartDate.Should().Be(startDate);
        record.EndDate.Should().BeNull();
        record.Status.Should().Be(EmploymentStatus.Active);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_Should_Throw_When_JobTitle_Is_Invalid(string? jobTitle)
    {
        var action = () => EmploymentRecord.Create(
            jobTitle!,
            EmploymentType.FullTime,
            DateOnly.FromDateTime(DateTime.UtcNow.Date));

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*JobTitle is required*");
    }

    [Fact]
    public void Create_Should_Throw_When_StartDate_Is_In_Future()
    {
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1));

        var action = () => EmploymentRecord.Create(
            "Care Assistant",
            EmploymentType.FullTime,
            futureDate);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*Start date cannot be in the future*");
    }

    [Fact]
    public void Terminate_Should_Set_EndDate_And_Status_To_Terminated()
    {
        var record = CreateEmploymentRecord();

        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        record.Terminate(endDate);

        record.EndDate.Should().Be(endDate);
        record.Status.Should().Be(EmploymentStatus.Terminated);
    }

    [Fact]
    public void Terminate_Should_Throw_When_EndDate_Is_Before_StartDate()
    {
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        var record = EmploymentRecord.Create(
            "Care Assistant",
            EmploymentType.FullTime,
            startDate);

        var invalidEndDate = startDate.AddDays(-1);

        var action = () => record.Terminate(invalidEndDate);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*End date cannot be before start date*");
    }

    [Fact]
    public void Terminate_Should_Throw_When_Already_Terminated()
    {
        var record = CreateEmploymentRecord();

        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        record.Terminate(endDate);

        var action = () => record.Terminate(endDate);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*Employment record is already terminated*");
    }

    private static EmploymentRecord CreateEmploymentRecord()
    {
        return EmploymentRecord.Create(
            "Care Assistant",
            EmploymentType.FullTime,
            DateOnly.FromDateTime(DateTime.UtcNow.Date));
    }
}