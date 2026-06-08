using CareWorkOps.Domain.Common;
using CareWorkOps.Domain.Staff;
using FluentAssertions;

namespace CareWorkOps.Domain.Tests.Staff;

public sealed class TrainingRecordTests
{
    [Fact]
    public void Create_Should_Create_Training_Record_With_Valid_Data()
    {
        var completedDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        var training = TrainingRecord.Create(
            "Medication Administration",
            TrainingType.Mandatory,
            completedDate,
            validityMonths: 12);

        training.TrainingName.Should().Be("Medication Administration");
        training.TrainingType.Should().Be(TrainingType.Mandatory);
        training.CompletedDate.Should().Be(completedDate);
        training.ExpiryDate.Should().Be(completedDate.AddMonths(12));
        training.Status.Should().Be(TrainingStatus.Valid);
        training.Provider.Should().BeNull();
        training.CertificateReference.Should().BeNull();
    }

    [Fact]
    public void Create_Should_Create_Training_Record_With_Provider_And_Certificate()
    {
        var completedDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        var training = TrainingRecord.Create(
            "Moving and Handling",
            TrainingType.Mandatory,
            completedDate,
            validityMonths: 12,
            provider: "NHS Learning Hub",
            certificateReference: "CERT-12345");

        training.Provider.Should().Be("NHS Learning Hub");
        training.CertificateReference.Should().Be("CERT-12345");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_Should_Throw_When_TrainingName_Is_Invalid(string? trainingName)
    {
        var action = () => TrainingRecord.Create(
            trainingName!,
            TrainingType.Mandatory,
            DateOnly.FromDateTime(DateTime.UtcNow.Date),
            validityMonths: 12);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*TrainingName is required*");
    }

    [Fact]
    public void Create_Should_Throw_When_CompletedDate_Is_In_Future()
    {
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1));

        var action = () => TrainingRecord.Create(
            "Medication Administration",
            TrainingType.Mandatory,
            futureDate,
            validityMonths: 12);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*Training completion date cannot be in the future*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_Should_Throw_When_ValidityMonths_Is_Invalid(int validityMonths)
    {
        var action = () => TrainingRecord.Create(
            "Medication Administration",
            TrainingType.Mandatory,
            DateOnly.FromDateTime(DateTime.UtcNow.Date),
            validityMonths);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*Training validity months must be greater than zero*");
    }

    [Fact]
    public void IsExpired_Should_Return_False_When_ExpiryDate_Is_Today()
    {
        var completedDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddMonths(-12));

        var training = TrainingRecord.Create(
            "Medication Administration",
            TrainingType.Mandatory,
            completedDate,
            validityMonths: 12);

        training.IsExpired().Should().BeFalse();
    }

    [Fact]
    public void IsExpired_Should_Return_True_When_ExpiryDate_Is_Before_Today()
    {
        var completedDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddMonths(-13));

        var training = TrainingRecord.Create(
            "Medication Administration",
            TrainingType.Mandatory,
            completedDate,
            validityMonths: 12);

        training.IsExpired().Should().BeTrue();
    }

    [Fact]
    public void MarkAsExpired_Should_Set_Status_To_Expired()
    {
        var training = CreateTrainingRecord();

        training.MarkAsExpired();

        training.Status.Should().Be(TrainingStatus.Expired);
    }

    [Fact]
    public void Renew_Should_Update_CompletedDate_ExpiryDate_And_Status()
    {
        var training = CreateTrainingRecord();

        training.MarkAsExpired();

        var newCompletedDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        training.Renew(
            newCompletedDate,
            validityMonths: 24,
            provider: "Care Training Academy",
            certificateReference: "CERT-RENEWED-001");

        training.CompletedDate.Should().Be(newCompletedDate);
        training.ExpiryDate.Should().Be(newCompletedDate.AddMonths(24));
        training.Status.Should().Be(TrainingStatus.Valid);
        training.Provider.Should().Be("Care Training Academy");
        training.CertificateReference.Should().Be("CERT-RENEWED-001");
    }

    [Fact]
    public void Renew_Should_Throw_When_NewCompletedDate_Is_In_Future()
    {
        var training = CreateTrainingRecord();

        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1));

        var action = () => training.Renew(
            futureDate,
            validityMonths: 12);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("*Training completion date cannot be in the future*");
    }

    private static TrainingRecord CreateTrainingRecord()
    {
        return TrainingRecord.Create(
            "Medication Administration",
            TrainingType.Mandatory,
            DateOnly.FromDateTime(DateTime.UtcNow.Date),
            validityMonths: 12);
    }
}