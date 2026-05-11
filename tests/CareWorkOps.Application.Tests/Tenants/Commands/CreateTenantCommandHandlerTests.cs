using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Abstractions.Persistence;
using CareWorkOps.Application.Tenants.Commands.CreateTenant;
using CareWorkOps.Domain.Tenants;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Application.Tests.Tenants.Commands
{
    public sealed class CreateTenantCommandHandlerTests
    {
        private readonly Mock<ITenantRepository> _tenantRepositoryMock = new();
        private readonly Mock<IIdentityService> _identityServiceMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

        private readonly CreateTenantCommandHandler _handler;

        public CreateTenantCommandHandlerTests()
        {
            _handler = new CreateTenantCommandHandler(
                _tenantRepositoryMock.Object,
                _identityServiceMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Create_Tenant_And_Admin_User_When_Command_Is_Valid()
        {
            // Arrange
            var command = CreateValidCommand();
            var adminUserId = Guid.NewGuid();

            _tenantRepositoryMock
                .Setup(x => x.ExistsBySlugAsync("alpha-care", It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _identityServiceMock
                .Setup(x => x.CreateTenantAdminAsync(
                    It.IsAny<Guid>(),
                    command.AdminFirstName,
                    command.AdminLastName,
                    command.AdminEmail,
                    command.AdminPassword,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateUserResult.Success(adminUserId));

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            result.Value.TenantId.Should().NotBeEmpty();
            result.Value.TenantName.Should().Be(command.TenantName);
            result.Value.TenantSlug.Should().Be("alpha-care");
            result.Value.IsolationMode.Should().Be(TenantIsolationMode.SharedDatabase);
            result.Value.Status.Should().Be(TenantStatus.PendingSetup);
            result.Value.AdminUserId.Should().Be(adminUserId);

            _tenantRepositoryMock.Verify(
                x => x.AddAsync(
                    It.Is<Tenant>(tenant =>
                        tenant.Name == command.TenantName &&
                        tenant.Slug == "alpha-care" &&
                        tenant.IsolationMode == TenantIsolationMode.SharedDatabase),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _identityServiceMock.Verify(
                x => x.CreateTenantAdminAsync(
                    result.Value.TenantId,
                    command.AdminFirstName,
                    command.AdminLastName,
                    command.AdminEmail,
                    command.AdminPassword,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Normalize_TenantSlug_To_Lowercase()
        {
            // Arrange
            var command = CreateValidCommand() with
            {
                TenantSlug = "ALPHA-CARE"
            };

            _tenantRepositoryMock
                .Setup(x => x.ExistsBySlugAsync("alpha-care", It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _identityServiceMock
                .Setup(x => x.CreateTenantAdminAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateUserResult.Success(Guid.NewGuid()));

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.TenantSlug.Should().Be("alpha-care");

            _tenantRepositoryMock.Verify(
                x => x.ExistsBySlugAsync("alpha-care", It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_When_TenantSlug_Already_Exists()
        {
            // Arrange
            var command = CreateValidCommand();

            _tenantRepositoryMock
                .Setup(x => x.ExistsBySlugAsync("alpha-care", It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Conflict.Error");
            result.Error.Message.Should().Contain("already exists");

            _tenantRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _identityServiceMock.Verify(
                x => x.CreateTenantAdminAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_When_IsolationMode_Is_Invalid()
        {
            // Arrange
            var command = CreateValidCommand() with
            {
                IsolationMode = "InvalidMode"
            };

            _tenantRepositoryMock
                .Setup(x => x.ExistsBySlugAsync("alpha-care", It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Validation.Error");
            result.Error.Message.Should().Be("Invalid tenant isolation mode.");

            _tenantRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _identityServiceMock.Verify(
                x => x.CreateTenantAdminAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_When_DedicatedDatabase_Has_No_ConnectionString()
        {
            // Arrange
            var command = CreateValidCommand() with
            {
                IsolationMode = "DedicatedDatabase",
                ConnectionString = null
            };

            _tenantRepositoryMock
                .Setup(x => x.ExistsBySlugAsync("alpha-care", It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Validation.Error");
            result.Error.Message.Should().Contain("Dedicated database tenants require a connection string");

            _tenantRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _identityServiceMock.Verify(
                x => x.CreateTenantAdminAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_When_Admin_User_Creation_Fails()
        {
            // Arrange
            var command = CreateValidCommand();

            _tenantRepositoryMock
                .Setup(x => x.ExistsBySlugAsync("alpha-care", It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _identityServiceMock
                .Setup(x => x.CreateTenantAdminAsync(
                    It.IsAny<Guid>(),
                    command.AdminFirstName,
                    command.AdminLastName,
                    command.AdminEmail,
                    command.AdminPassword,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateUserResult.Failure([
                    "Email already exists.",
                "Password is too weak."
                ]));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Failure.Error");
            result.Error.Message.Should().Contain("Email already exists.");
            result.Error.Message.Should().Contain("Password is too weak.");

            _tenantRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Create_DedicatedDatabase_Tenant_When_ConnectionString_Is_Provided()
        {
            // Arrange
            const string connectionString =
                "Server=localhost;Database=AlphaCareDb;User Id=sa;Password=Password123!;TrustServerCertificate=True;";

            var command = CreateValidCommand() with
            {
                IsolationMode = "DedicatedDatabase",
                ConnectionString = connectionString
            };

            _tenantRepositoryMock
                .Setup(x => x.ExistsBySlugAsync("alpha-care", It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _identityServiceMock
                .Setup(x => x.CreateTenantAdminAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateUserResult.Success(Guid.NewGuid()));

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.IsolationMode.Should().Be(TenantIsolationMode.DedicatedDatabase);

            _tenantRepositoryMock.Verify(
                x => x.AddAsync(
                    It.Is<Tenant>(tenant =>
                        tenant.IsolationMode == Domain.Tenants.TenantIsolationMode.DedicatedDatabase &&
                        tenant.ConnectionString == connectionString),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private static CreateTenantCommand CreateValidCommand()
        {
            return new CreateTenantCommand(
                TenantName: "Alpha Care Ltd",
                TenantSlug: "alpha-care",
                AdminFirstName: "John",
                AdminLastName: "Smith",
                AdminEmail: "admin@alphacare.com",
                AdminPassword: "Password123!",
                IsolationMode: "SharedDatabase");
        }
    }
}
