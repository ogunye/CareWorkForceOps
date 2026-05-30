using CareWorkOps.Application.Abstractions.Auditing;
using CareWorkOps.Application.Abstractions.Persistence;
using CareWorkOps.Application.Tenants.Commands.ActivateTenant;
using CareWorkOps.Application.Tenants.Commands.ArchiveTenant;
using CareWorkOps.Application.Tenants.Commands.SuspendTenant;
using CareWorkOps.Application.Tenants.Commands.UpdateTenant;
using CareWorkOps.Application.Tenants.Queries.GetTenantById;
using CareWorkOps.Application.Tenants.Queries.GetTenantBySlug;
using CareWorkOps.Domain.Tenants;
using FluentAssertions;
using Moq;

namespace CareWorkOps.Application.Tests.Tenants.TenantLifecycle;

public sealed class TenantLifecycleHandlerTests
{
    private readonly Mock<ITenantRepository> _tenantRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IAuditService> _auditServiceMock = new();

    [Fact]
    public async Task ActivateTenant_Should_Return_Success()
    {
        var tenant = CreateTenant();

        _tenantRepositoryMock
            .Setup(x => x.GetByIdAsync(tenant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new ActivateTenantCommandHandler(
            _tenantRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _auditServiceMock.Object);

        var result = await handler.Handle(
            new ActivateTenantCommand(tenant.Id.Value),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        tenant.Status.Should().Be(TenantStatus.Active);

        _auditServiceMock.Verify(x => x.RecordAsync(
            tenant.Id.Value,
            null,
            "TenantActivated",
            "Tenant",
            tenant.Id.Value.ToString(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SuspendTenant_Should_Return_Success()
    {
        var tenant = CreateTenant();

        _tenantRepositoryMock
            .Setup(x => x.GetByIdAsync(tenant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new SuspendTenantCommandHandler(
            _tenantRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _auditServiceMock.Object);

        var result = await handler.Handle(
            new SuspendTenantCommand(tenant.Id.Value, "Compliance issue."),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        tenant.Status.Should().Be(TenantStatus.Suspended);

        _auditServiceMock.Verify(x => x.RecordAsync(
            tenant.Id.Value,
            null,
            "TenantSuspended",
            "Tenant",
            tenant.Id.Value.ToString(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ArchiveTenant_Should_Return_Success()
    {
        var tenant = CreateTenant();

        _tenantRepositoryMock
            .Setup(x => x.GetByIdAsync(tenant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new ArchiveTenantCommandHandler(
            _tenantRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _auditServiceMock.Object);

        var result = await handler.Handle(
            new ArchiveTenantCommand(tenant.Id.Value),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        tenant.Status.Should().Be(TenantStatus.Archived);

        _auditServiceMock.Verify(x => x.RecordAsync(
            tenant.Id.Value,
            null,
            "TenantArchived",
            "Tenant",
            tenant.Id.Value.ToString(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTenant_Should_Return_Success()
    {
        var tenant = CreateTenant();

        _tenantRepositoryMock
            .Setup(x => x.GetByIdAsync(tenant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateTenantCommandHandler(
            _tenantRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _auditServiceMock.Object);

        var command = new UpdateTenantCommand(
            tenant.Id.Value,
            "Updated Care Ltd",
            "SharedDatabase",
            null);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Updated Care Ltd");

        _auditServiceMock.Verify(x => x.RecordAsync(
            tenant.Id.Value,
            null,
            "TenantUpdated",
            "Tenant",
            tenant.Id.Value.ToString(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTenantById_Should_Return_Tenant()
    {
        var tenant = CreateTenant();

        _tenantRepositoryMock
            .Setup(x => x.GetByIdAsync(tenant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);

        var handler = new GetTenantByIdQueryHandler(_tenantRepositoryMock.Object);

        var result = await handler.Handle(
            new GetTenantByIdQuery(tenant.Id.Value),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(tenant.Id.Value);
    }

    [Fact]
    public async Task GetTenantBySlug_Should_Return_Tenant()
    {
        var tenant = CreateTenant();

        _tenantRepositoryMock
            .Setup(x => x.GetBySlugAsync("gamma-care", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);

        var handler = new GetTenantBySlugQueryHandler(_tenantRepositoryMock.Object);

        var result = await handler.Handle(
            new GetTenantBySlugQuery("gamma-care"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Slug.Should().Be("gamma-care");
    }

    private static Tenant CreateTenant()
    {
        return Tenant.Create(
            "Gamma Care Ltd",
            "gamma-care",
            TenantIsolationMode.SharedDatabase);
    }
}