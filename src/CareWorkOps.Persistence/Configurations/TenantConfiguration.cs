using CareWorkOps.Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Persistence.Configurations
{
    public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTable("Tenants");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasConversion(
                    tenantId => tenantId.Value,
                    value => TenantId.From(value))
                .ValueGeneratedNever();

            builder.Property(x => x.TenantId)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.Slug)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.IsolationMode)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.ConnectionString)
                .HasMaxLength(2000);

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired();

            builder.Property(x => x.CreatedByUserId);

            builder.Property(x => x.UpdatedAtUtc);

            builder.Property(x => x.UpdatedByUserId);

            builder.Property(x => x.DeletedAtUtc);

            builder.Property(x => x.DeletedByUserId);

            builder.Property(x => x.IsDeleted)
                .IsRequired();

            builder.Property(x => x.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            builder.HasIndex(x => x.Slug)
                .IsUnique();

            builder.HasIndex(x => x.TenantId);

            builder.HasIndex(x => x.Status);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
