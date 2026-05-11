using CareWorkOps.Domain.Common;
using CareWorkOps.Domain.Tenants;
using CareWorkOps.Domain.Tenants.Events;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Domain.Tests.Tenants
{
    public sealed class TenantTests
    {
        [Fact]
        public void Create_Should_Create_Tenant_With_Valid_Data()
        {
            // Act
            var tenant = Tenant.Create(
                name: "Alpha Care Ltd",
                slug: "alpha-care",
                isolationMode: TenantIsolationMode.SharedDatabase);

            // Assert
            tenant.Id.Value.Should().NotBeEmpty();
            tenant.Name.Should().Be("Alpha Care Ltd");
            tenant.Slug.Should().Be("alpha-care");
            tenant.IsolationMode.Should().Be(TenantIsolationMode.SharedDatabase);
            tenant.Status.Should().Be(TenantStatus.PendingSetup);
            tenant.ConnectionString.Should().BeNull();
            tenant.IsDeleted.Should().BeFalse();
            tenant.TenantId.Should().Be(tenant.Id.Value);
            tenant.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_Should_Throw_When_Name_Is_Invalid(string? name)
        {
            // Act
            var action = () => Tenant.Create(
                name: name!,
                slug: "alpha-care",
                isolationMode: TenantIsolationMode.SharedDatabase);

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage("*name is required*");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_Should_Throw_When_Slug_Is_Empty(string? slug)
        {
            // Act
            var action = () => Tenant.Create(
                name: "Alpha Care Ltd",
                slug: slug!,
                isolationMode: TenantIsolationMode.SharedDatabase);

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage("*slug is required*");
        }

        [Theory]
        [InlineData("Alpha Care")]
        [InlineData("alpha_care")]
        [InlineData("alpha--care")]
        [InlineData("-alpha-care")]
        [InlineData("alpha-care-")]
        [InlineData("alpha.care")]
        [InlineData("alpha@care")]
        public void Create_Should_Throw_When_Slug_Format_Is_Invalid(string slug)
        {
            // Act
            var action = () => Tenant.Create(
                name: "Alpha Care Ltd",
                slug: slug,
                isolationMode: TenantIsolationMode.SharedDatabase);

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage(TenantErrors.InvalidSlug);
        }

        [Fact]
        public void Create_Should_Normalize_Slug_To_Lowercase()
        {
            // Act
            var tenant = Tenant.Create(
                name: "Alpha Care Ltd",
                slug: "ALPHA-CARE",
                isolationMode: TenantIsolationMode.SharedDatabase);

            // Assert
            tenant.Slug.Should().Be("alpha-care");
        }

        [Fact]
        public void Create_Should_Require_ConnectionString_When_IsolationMode_Is_DedicatedDatabase()
        {
            // Act
            var action = () => Tenant.Create(
                name: "Enterprise Care Ltd",
                slug: "enterprise-care",
                isolationMode: TenantIsolationMode.DedicatedDatabase);

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage(TenantErrors.DedicatedDatabaseRequiresConnectionString);
        }

        [Fact]
        public void Create_Should_Allow_DedicatedDatabase_When_ConnectionString_Is_Provided()
        {
            // Arrange
            const string connectionString =
                "Server=localhost;Database=EnterpriseCareDb;User Id=sa;Password=Password123!;TrustServerCertificate=True;";

            // Act
            var tenant = Tenant.Create(
                name: "Enterprise Care Ltd",
                slug: "enterprise-care",
                isolationMode: TenantIsolationMode.DedicatedDatabase,
                connectionString: connectionString);

            // Assert
            tenant.IsolationMode.Should().Be(TenantIsolationMode.DedicatedDatabase);
            tenant.ConnectionString.Should().Be(connectionString);
        }

        [Fact]
        public void Create_Should_Raise_TenantCreatedDomainEvent()
        {
            // Act
            var tenant = Tenant.Create(
                name: "Alpha Care Ltd",
                slug: "alpha-care",
                isolationMode: TenantIsolationMode.SharedDatabase);

            // Assert
            tenant.DomainEvents.Should().ContainSingle();

            var domainEvent = tenant.DomainEvents
                .Should()
                .ContainSingle(x => x is TenantCreatedDomainEvent)
                .Subject
                .As<TenantCreatedDomainEvent>();

            domainEvent.TenantId.Should().Be(tenant.Id.Value);
            domainEvent.Name.Should().Be("Alpha Care Ltd");
            domainEvent.Slug.Should().Be("alpha-care");
            domainEvent.OccurredOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Activate_Should_Change_Status_To_Active()
        {
            // Arrange
            var tenant = CreateDefaultTenant();

            // Act
            tenant.Activate();

            // Assert
            tenant.Status.Should().Be(TenantStatus.Active);
        }

        [Fact]
        public void Activate_Should_Raise_TenantActivatedDomainEvent()
        {
            // Arrange
            var tenant = CreateDefaultTenant();
            tenant.ClearDomainEvents();

            // Act
            tenant.Activate();

            // Assert
            tenant.DomainEvents.Should().ContainSingle();

            var domainEvent = tenant.DomainEvents
                .Should()
                .ContainSingle(x => x is TenantActivatedDomainEvent)
                .Subject
                .As<TenantActivatedDomainEvent>();

            domainEvent.TenantId.Should().Be(tenant.Id.Value);
            domainEvent.OccurredOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Activate_Should_Throw_When_Tenant_Is_Already_Active()
        {
            // Arrange
            var tenant = CreateDefaultTenant();
            tenant.Activate();

            // Act
            var action = tenant.Activate;

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage(TenantErrors.AlreadyActive);
        }

        [Fact]
        public void Suspend_Should_Change_Status_To_Suspended()
        {
            // Arrange
            var tenant = CreateDefaultTenant();

            // Act
            tenant.Suspend("Compliance issue under review.");

            // Assert
            tenant.Status.Should().Be(TenantStatus.Suspended);
        }

        [Fact]
        public void Suspend_Should_Raise_TenantSuspendedDomainEvent()
        {
            // Arrange
            var tenant = CreateDefaultTenant();
            tenant.ClearDomainEvents();

            // Act
            tenant.Suspend("Compliance issue under review.");

            // Assert
            tenant.DomainEvents.Should().ContainSingle();

            var domainEvent = tenant.DomainEvents
                .Should()
                .ContainSingle(x => x is TenantSuspendedDomainEvent)
                .Subject
                .As<TenantSuspendedDomainEvent>();

            domainEvent.TenantId.Should().Be(tenant.Id.Value);
            domainEvent.Reason.Should().Be("Compliance issue under review.");
            domainEvent.OccurredOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Suspend_Should_Throw_When_Tenant_Is_Already_Suspended()
        {
            // Arrange
            var tenant = CreateDefaultTenant();
            tenant.Suspend("First suspension.");

            // Act
            var action = () => tenant.Suspend("Second suspension.");

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage(TenantErrors.AlreadySuspended);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Suspend_Should_Throw_When_Reason_Is_Invalid(string? reason)
        {
            // Arrange
            var tenant = CreateDefaultTenant();

            // Act
            var action = () => tenant.Suspend(reason!);

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage("*reason is required*");
        }

        [Fact]
        public void Rename_Should_Update_Tenant_Name()
        {
            // Arrange
            var tenant = CreateDefaultTenant();

            // Act
            tenant.Rename("Beta Care Ltd");

            // Assert
            tenant.Name.Should().Be("Beta Care Ltd");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Rename_Should_Throw_When_Name_Is_Invalid(string? name)
        {
            // Arrange
            var tenant = CreateDefaultTenant();

            // Act
            var action = () => tenant.Rename(name!);

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage("*name is required*");
        }

        [Fact]
        public void Archive_Should_Change_Status_To_Archived()
        {
            // Arrange
            var tenant = CreateDefaultTenant();

            // Act
            tenant.Archive();

            // Assert
            tenant.Status.Should().Be(TenantStatus.Archived);
        }

        [Fact]
        public void Rename_Should_Throw_When_Tenant_Is_Archived()
        {
            // Arrange
            var tenant = CreateDefaultTenant();
            tenant.Archive();

            // Act
            var action = () => tenant.Rename("Archived Care Ltd");

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage(TenantErrors.ArchivedTenantCannotBeModified);
        }

        [Fact]
        public void Activate_Should_Throw_When_Tenant_Is_Archived()
        {
            // Arrange
            var tenant = CreateDefaultTenant();
            tenant.Archive();

            // Act
            var action = tenant.Activate;

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage(TenantErrors.ArchivedTenantCannotBeModified);
        }

        [Fact]
        public void Suspend_Should_Throw_When_Tenant_Is_Archived()
        {
            // Arrange
            var tenant = CreateDefaultTenant();
            tenant.Archive();

            // Act
            var action = () => tenant.Suspend("Suspension after archive.");

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage(TenantErrors.ArchivedTenantCannotBeModified);
        }

        [Fact]
        public void ChangeIsolationMode_Should_Update_Isolation_Mode()
        {
            // Arrange
            var tenant = CreateDefaultTenant();

            // Act
            tenant.ChangeIsolationMode(TenantIsolationMode.SchemaPerTenant);

            // Assert
            tenant.IsolationMode.Should().Be(TenantIsolationMode.SchemaPerTenant);
            tenant.ConnectionString.Should().BeNull();
        }

        [Fact]
        public void ChangeIsolationMode_Should_Require_ConnectionString_For_DedicatedDatabase()
        {
            // Arrange
            var tenant = CreateDefaultTenant();

            // Act
            var action = () => tenant.ChangeIsolationMode(TenantIsolationMode.DedicatedDatabase);

            // Assert
            action.Should()
                .Throw<DomainException>()
                .WithMessage(TenantErrors.DedicatedDatabaseRequiresConnectionString);
        }

        [Fact]
        public void ChangeIsolationMode_Should_Set_ConnectionString_For_DedicatedDatabase()
        {
            // Arrange
            var tenant = CreateDefaultTenant();

            const string connectionString =
                "Server=localhost;Database=AlphaCareDb;User Id=sa;Password=Password123!;TrustServerCertificate=True;";

            // Act
            tenant.ChangeIsolationMode(
                TenantIsolationMode.DedicatedDatabase,
                connectionString);

            // Assert
            tenant.IsolationMode.Should().Be(TenantIsolationMode.DedicatedDatabase);
            tenant.ConnectionString.Should().Be(connectionString);
        }

        [Fact]
        public void ClearDomainEvents_Should_Remove_All_DomainEvents()
        {
            // Arrange
            var tenant = CreateDefaultTenant();

            // Act
            tenant.ClearDomainEvents();

            // Assert
            tenant.DomainEvents.Should().BeEmpty();
        }

        private static Tenant CreateDefaultTenant()
        {
            return Tenant.Create(
                name: "Alpha Care Ltd",
                slug: "alpha-care",
                isolationMode: TenantIsolationMode.SharedDatabase);
        }
    }

}
