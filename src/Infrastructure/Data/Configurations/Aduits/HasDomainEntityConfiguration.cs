using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RuanFa.FashionShop.SharedKernel.Interfaces.Domain;

namespace RuanFa.FashionShop.Infrastructure.Data.Configurations.Aduits;
internal class HasDomainEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class, IHasDomainEvent
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Ignore Domain Events
        builder.Ignore(ca => ca.DomainEvents);
    }
}
