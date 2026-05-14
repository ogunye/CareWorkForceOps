using CareWorkOps.Application.Abstractions.Persistence;
using CareWorkOps.Domain.Tenants;
using CareWorkOps.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Persistence.Repositories
{
    public sealed class TenantRepository : ITenantRepository
    {
        private readonly ApplicationDbContext _context;

        public TenantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default)
        {
            return await _context.Tenants
                .AnyAsync(x => x.Slug == slug, cancellationToken);
        }

        public async Task AddAsync(
            Tenant tenant,
            CancellationToken cancellationToken = default)
        {
            await _context.Tenants.AddAsync(tenant, cancellationToken);
        }

        public async Task<Tenant?> GetByIdAsync(
            TenantId tenantId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Tenants
                .FirstOrDefaultAsync(x => x.Id == tenantId, cancellationToken);
        }

        public async Task<Tenant?> GetBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default)
        {
            return await _context.Tenants
                .FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);
        }
    }
}
