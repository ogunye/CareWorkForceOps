using CareWorkOps.Application.Abstractions.Auditing;
using CareWorkOps.Application.Auditing.Dtos;
using CareWorkOps.Application.Auditing.Queries.GetAuditLogById;
using CareWorkOps.Application.Auditing.Queries.GetAuditLogs;
using FluentAssertions;
using Moq;

namespace CareWorkOps.Application.Tests.Auditing.AuditLogs;

public sealed class AuditLogQueryHandlerTests
{
    private readonly Mock<IAuditLogService> _auditLogServiceMock = new();

    [Fact]
    public async Task GetAuditLogs_Should_Return_AuditLogs()
    {
        var tenantId = Guid.NewGuid();

        _auditLogServiceMock
            .Setup(x => x.GetAuditLogsAsync(tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new AuditLogDto(
                    Guid.NewGuid(),
                    tenantId,
                    Guid.NewGuid(),
                    "UserCreated",
                    "ApplicationUser",
                    Guid.NewGuid().ToString(),
                    "User was created.",
                    DateTime.UtcNow)
            ]);

        var handler = new GetAuditLogsQueryHandler(_auditLogServiceMock.Object);

        var result = await handler.Handle(
            new GetAuditLogsQuery(tenantId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
    }

    [Fact]
    public async Task GetAuditLogById_Should_Return_AuditLog_When_Found()
    {
        var tenantId = Guid.NewGuid();
        var auditLogId = Guid.NewGuid();

        _auditLogServiceMock
            .Setup(x => x.GetAuditLogByIdAsync(
                tenantId,
                auditLogId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditLogDto(
                auditLogId,
                tenantId,
                Guid.NewGuid(),
                "RoleCreated",
                "ApplicationRole",
                Guid.NewGuid().ToString(),
                "Role was created.",
                DateTime.UtcNow));

        var handler = new GetAuditLogByIdQueryHandler(_auditLogServiceMock.Object);

        var result = await handler.Handle(
            new GetAuditLogByIdQuery(tenantId, auditLogId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(auditLogId);
    }

    [Fact]
    public async Task GetAuditLogById_Should_Return_Failure_When_Not_Found()
    {
        var tenantId = Guid.NewGuid();
        var auditLogId = Guid.NewGuid();

        _auditLogServiceMock
            .Setup(x => x.GetAuditLogByIdAsync(
                tenantId,
                auditLogId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((AuditLogDto?)null);

        var handler = new GetAuditLogByIdQueryHandler(_auditLogServiceMock.Object);

        var result = await handler.Handle(
            new GetAuditLogByIdQuery(tenantId, auditLogId),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("Audit log was not found.");
    }
}