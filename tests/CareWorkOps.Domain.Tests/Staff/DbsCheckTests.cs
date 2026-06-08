using CareWorkOps.Domain.Common;
using CareWorkOps.Domain.Staff;
using FluentAssertions;

namespace CareWorkOps.Domain.Tests.Staff;

public sealed class DbsCheckTests
{
    [Fact]
    public void Create_Should_Create_DbsCheck_With_Valid_Data()
    {
        var issueDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        var dbsCheck = DbsCheck.Create(
            "DBS-123456",
            DbsCheckType.Enhanced,
            issueDate);

        dbsCheck.CertificateNumber.Should().Be("DBS-123456");
        dbsCheck.CheckType.Should().Be(DbsCheckType.Enhanced);
        dbsCheck.IssueDate.Should().Be(issueDate);
        dbsCheck.Status.Should().Be(DbsCheckStatus.Clear);
        dbsCheck.ReviewDate.Should().Be(issueDate.AddYears(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_Should_Throw_When_CertificateNumber_Is_Invalid(string? certificateNumber)
    {
        var action = () => DbsCheck.Create(
            certificateNumber!,
            DbsCheckType.Enhanced,
            DateOnly.FromDateTime(DateTime.UtcNow.Date));

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*CertificateNumber is required*");
    }

    [Fact]
    public void Create_Should_Throw_When_IssueDate_Is_In_Future()
    {
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1));

        var action = () => DbsCheck.Create(
            "DBS-123456",
            DbsCheckType.Enhanced,
            futureDate);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*DBS issue date cannot be in the future*");
    }

    [Fact]
    public void MarkAsUnderReview_Should_Set_Status_To_UnderReview()
    {
        var dbsCheck = CreateDbsCheck();

        dbsCheck.MarkAsUnderReview("Employer is reviewing update service result.");

        dbsCheck.Status.Should().Be(DbsCheckStatus.UnderReview);
        dbsCheck.ReviewNotes.Should().Be("Employer is reviewing update service result.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void MarkAsUnderReview_Should_Throw_When_Notes_Are_Invalid(string? notes)
    {
        var dbsCheck = CreateDbsCheck();

        var action = () => dbsCheck.MarkAsUnderReview(notes!);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*ReviewNotes is required*");
    }

    [Fact]
    public void MarkAsNotClear_Should_Set_Status_And_Notes()
    {
        var dbsCheck = CreateDbsCheck();

        dbsCheck.MarkAsNotClear("Relevant disclosure requires management decision.");

        dbsCheck.Status.Should().Be(DbsCheckStatus.NotClear);
        dbsCheck.ReviewNotes.Should().Be("Relevant disclosure requires management decision.");
    }

    [Fact]
    public void Renew_Should_Update_Details_And_Clear_Status()
    {
        var dbsCheck = CreateDbsCheck();
        var newIssueDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        dbsCheck.MarkAsUnderReview("Old check under review.");

        dbsCheck.Renew(
            "DBS-654321",
            DbsCheckType.Enhanced,
            newIssueDate);

        dbsCheck.CertificateNumber.Should().Be("DBS-654321");
        dbsCheck.CheckType.Should().Be(DbsCheckType.Enhanced);
        dbsCheck.IssueDate.Should().Be(newIssueDate);
        dbsCheck.Status.Should().Be(DbsCheckStatus.Clear);
        dbsCheck.ReviewNotes.Should().BeNull();
        dbsCheck.ReviewDate.Should().Be(newIssueDate.AddYears(1));
    }

    [Fact]
    public void IsExpired_Should_Return_True_When_ReviewDate_Is_Before_Today()
    {
        var oldIssueDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddYears(-2));

        var dbsCheck = DbsCheck.Create(
            "DBS-OLD",
            DbsCheckType.Enhanced,
            oldIssueDate);

        dbsCheck.IsExpired().Should().BeTrue();
    }

    private static DbsCheck CreateDbsCheck()
    {
        return DbsCheck.Create(
            "DBS-123456",
            DbsCheckType.Enhanced,
            DateOnly.FromDateTime(DateTime.UtcNow.Date));
    }
}