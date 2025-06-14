using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.FashionShop.SharedKernel.Interfaces.Domain;

namespace RuanFa.FashionShop.Infrastructure.Data.Configurations.Aduits;

internal sealed class AuditableEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class, IAuditable, IHasDomainEvent
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.LastModified);

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(200);

        builder.Property(e => e.CreatedAt)
            .HasMaxLength(200);

        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.CreatedBy);
        builder.HasIndex(e => e.LastModifiedBy);

        // Ignore Domain Events
        builder.Ignore(ca => ca.DomainEvents);
    }
}
