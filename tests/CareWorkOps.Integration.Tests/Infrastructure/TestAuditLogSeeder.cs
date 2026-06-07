using CareWorkOps.Persistence;
using CareWorkOps.Persistence.Auditing;
using CareWorkOps.Persistence.Context;

namespace CareWorkOps.Api.IntegrationTests.Infrastructure;

public static class TestAuditLogSeeder
{
    public static async Task<Guid> SeedAuditLogAsync(
        ApplicationDbContext dbContext,
        Guid? tenantId = null)
    {
        var auditLog = new AuditLog(
            tenantId: tenantId ?? Guid.Parse(TestClaimsProvider.TenantId),
            userId: Guid.Parse(TestClaimsProvider.UserId),
            action: "LoginSuccess",
            entityName: "User",
            entityId: TestClaimsProvider.UserId,
            description: "Integration test audit log"
        );

        dbContext.AuditLogs.Add(auditLog);
        await dbContext.SaveChangesAsync();

        return auditLog.Id;
    }
}