using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.FashionShop.Infrastructure.Data.Constants;

namespace RuanFa.FashionShop.Infrastructure.Data.Configurations.Identities;

internal class IdentityRoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
    {
        builder.ToTable(Schema.RoleClaims);
    }
}
